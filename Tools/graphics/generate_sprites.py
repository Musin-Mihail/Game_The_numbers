"""Запасной генератор PNG без GPU. Основной путь — Tools/comfyui/generate.py.

Запуск из корня репозитория:
    python Tools/graphics/generate_sprites.py
"""

from __future__ import annotations

import math
import struct
import sys
import zlib
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import style as S


def write_png(path: Path, width: int, height: int, pixels: bytearray) -> None:
    raw = bytearray()
    row = width * 4
    for y in range(height):
        raw.append(0)
        raw.extend(pixels[y * row : (y + 1) * row])

    def chunk(tag: bytes, data: bytes) -> bytes:
        return struct.pack(">I", len(data)) + tag + data + struct.pack(
            ">I", zlib.crc32(tag + data) & 0xFFFFFFFF
        )

    ihdr = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    png = b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", zlib.compress(bytes(raw), 9)) + chunk(b"IEND", b"")
    path.write_bytes(png)


def blank(width: int, height: int, color=(0, 0, 0, 0)) -> bytearray:
    px = bytearray(width * height * 4)
    r, g, b, a = color
    for i in range(0, len(px), 4):
        px[i : i + 4] = bytes((r, g, b, a))
    return px


def set_px(px: bytearray, width: int, x: int, y: int, color, height: int) -> None:
    if x < 0 or y < 0 or x >= width or y >= height:
        return
    i = (y * width + x) * 4
    px[i : i + 4] = bytes(color)


def blend_px(px: bytearray, width: int, x: int, y: int, color, height: int) -> None:
    if x < 0 or y < 0 or x >= width or y >= height:
        return
    nr, ng, nb, na = color
    if na <= 0:
        return
    i = (y * width + x) * 4
    or_, og, ob, oa = px[i], px[i + 1], px[i + 2], px[i + 3]
    if na >= 255 or oa == 0:
        px[i : i + 4] = bytes((nr, ng, nb, na))
        return
    a = na / 255.0
    inv = 1.0 - a
    px[i] = int(nr * a + or_ * inv)
    px[i + 1] = int(ng * a + og * inv)
    px[i + 2] = int(nb * a + ob * inv)
    px[i + 3] = min(255, oa + na)


def coverage_fill(inside_fn, width: int, height: int, color) -> bytearray:
    px = blank(width, height)
    r, g, b = color[0], color[1], color[2]
    samples = (0.2, 0.5, 0.8)
    for y in range(height):
        for x in range(width):
            hit = 0
            for ox in samples:
                for oy in samples:
                    if inside_fn(x + ox, y + oy):
                        hit += 1
            if hit == 0:
                continue
            a = int(round(255 * hit / 9))
            set_px(px, width, x, y, (r, g, b, a), height)
    return px


def rect(px, w, h, x0, y0, x1, y1, color):
    for y in range(int(y0), int(y1) + 1):
        for x in range(int(x0), int(x1) + 1):
            set_px(px, w, x, y, color, h)


def rounded_rect_inside(x, y, x0, y0, x1, y1, radius):
    if x < x0 or x > x1 or y < y0 or y > y1:
        return False
    cx = min(max(x, x0 + radius), x1 - radius)
    cy = min(max(y, y0 + radius), y1 - radius)
    if abs(x - cx) <= radius and abs(y - cy) <= radius:
        return (x - cx) ** 2 + (y - cy) ** 2 <= radius * radius
    return True


def draw_menu(n):
    def inside(x, y):
        m = n * 0.22
        bar_h = n * 0.08
        gap = n * 0.14
        left, right = m, n - m
        for i in range(3):
            top = n * 0.28 + i * (bar_h + gap)
            if left <= x <= right and top <= y <= top + bar_h:
                return True
        return False

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_add(n):
    def inside(x, y):
        cx = cy = n / 2
        t = n * 0.09
        arm = n * 0.32
        return (abs(x - cx) <= t and abs(y - cy) <= arm) or (abs(y - cy) <= t and abs(x - cx) <= arm)

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_close(n):
    def inside(x, y):
        cx = cy = n / 2
        t = n * 0.08
        half = n * 0.28
        dx, dy = x - cx, y - cy
        r1 = abs(dx - dy) / math.sqrt(2)
        r2 = abs(dx + dy) / math.sqrt(2)
        along1 = abs(dx + dy) / math.sqrt(2)
        along2 = abs(dx - dy) / math.sqrt(2)
        return (r1 <= t and along1 <= half * math.sqrt(2)) or (r2 <= t and along2 <= half * math.sqrt(2))

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_play(n):
    def inside(x, y):
        x0, y0 = n * 0.22, n * 0.16
        x1, y1 = n * 0.22, n * 0.84
        x2, y2 = n * 0.84, n * 0.50

        def sign(ax, ay, bx, by, cx, cy):
            return (ax - cx) * (by - cy) - (bx - cx) * (ay - cy)

        b1 = sign(x, y, x0, y0, x1, y1) < 0
        b2 = sign(x, y, x1, y1, x2, y2) < 0
        b3 = sign(x, y, x2, y2, x0, y0) < 0
        return b1 == b2 == b3

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_gear(n):
    def inside(x, y):
        cx = cy = (n - 1) / 2
        dx, dy = x - cx, y - cy
        r = math.hypot(dx, dy)
        ang = math.atan2(dy, dx)
        teeth = 8
        tooth_w = 0.38
        phase = (ang / (2 * math.pi) * teeth) % 1.0
        dist_tooth = min(phase, 1 - phase) * 2
        r_hole = n * 0.16
        r_hub = n * 0.38
        r_tooth = n * 0.46
        in_hole = r <= r_hole
        in_body = n * 0.28 <= r <= r_hub
        in_tooth = r_hub < r <= r_tooth and dist_tooth < tooth_w
        return (not in_hole) and (in_body or in_tooth)

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_checkbox(n):
    def inside(x, y):
        # галка: две линии
        x0, y0 = n * 0.22, n * 0.52
        x1, y1 = n * 0.42, n * 0.72
        x2, y2 = n * 0.78, n * 0.28
        t = n * 0.07

        def dist_seg(px, py, ax, ay, bx, by):
            vx, vy = bx - ax, by - ay
            l2 = vx * vx + vy * vy
            if l2 == 0:
                return math.hypot(px - ax, py - ay)
            tpar = max(0.0, min(1.0, ((px - ax) * vx + (py - ay) * vy) / l2))
            return math.hypot(px - (ax + tpar * vx), py - (ay + tpar * vy))

        return dist_seg(x, y, x0, y0, x1, y1) <= t or dist_seg(x, y, x1, y1, x2, y2) <= t

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_hint(n):
    def inside(x, y):
        cx = n / 2
        if math.hypot(x - cx, y - n * 0.82) <= n * 0.055:
            return True
        if abs(x - cx) <= n * 0.05 and n * 0.54 <= y <= n * 0.68:
            return True
        qcx, qcy, r = cx, n * 0.34, n * 0.18
        d = math.hypot(x - qcx, y - qcy)
        if abs(d - r) > n * 0.055:
            return False
        ang = math.atan2(qcy - y, x - qcx)
        # вырезаем низ дуги — там начинается ножка
        return not (-2.15 < ang < -0.95)

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_undo(n):
    def inside(x, y):
        cx, cy = n * 0.52, n * 0.52
        r = n * 0.26
        t = n * 0.07
        dx, dy = x - cx, y - cy
        d = math.hypot(dx, dy)
        ang = math.atan2(dy, dx)
        # дуга ~ 250 градусов, разрыв справа-сверху
        on_ring = abs(d - r) <= t and not (-0.35 < ang < 0.85)
        # наконечник
        tip_x, tip_y = cx + r * math.cos(-0.2), cy + r * math.sin(-0.2)
        hx, hy = x - tip_x, y - tip_y
        arrow = math.hypot(hx, hy) < n * 0.12 and hy < n * 0.04 and hx > -n * 0.08
        return on_ring or arrow

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_rating(n):
    def inside(x, y):
        bars = ((0.22, 0.62, 0.18), (0.42, 0.48, 0.18), (0.62, 0.32, 0.18))
        for left_f, top_f, w_f in bars:
            left, top, bw = n * left_f, n * top_f, n * w_f
            if left <= x <= left + bw and top <= y <= n * 0.82:
                return True
        # звезда-ромб над средней
        sx, sy = n * 0.51, n * 0.22
        return abs(x - sx) / (n * 0.08) + abs(y - sy) / (n * 0.08) <= 1

    return coverage_fill(inside, n, n, (*S.ICON_RGB, 255))


def draw_cell(n):
    pad = n * 0.06
    radius = n * 0.12

    def inside(x, y):
        return rounded_rect_inside(x, y, pad, pad, n - pad, n - pad, radius)

    return coverage_fill(inside, n, n, S.CELL_FILL)


def draw_background():
    w, h = S.BACKGROUND_WIDTH, S.BACKGROUND_HEIGHT
    px = blank(w, h, S.BACKGROUND)
    col_w = w / S.GRID_COLUMNS
    # вертикальные линии 10 колонок, очень бледные
    for c in range(S.GRID_COLUMNS + 1):
        x = int(round(c * col_w))
        for y in range(h):
            blend_px(px, w, x, y, S.GRID_LINE, h)
    return px, w, h


def out_dir() -> Path:
    return Path(__file__).resolve().parents[2] / "Assets" / "Resources" / "Sprites"


def main() -> None:
    dest = out_dir()
    dest.mkdir(parents=True, exist_ok=True)
    n = S.ICON_SIZE
    drawers = {
        "menu": draw_menu,
        "add": draw_add,
        "undo": draw_undo,
        "hint": draw_hint,
        "Rating": draw_rating,
        "Close": draw_close,
        "CheckBox": draw_checkbox,
        "gear": draw_gear,
        "play": draw_play,
        "Cell": draw_cell,
    }
    for name, fn in drawers.items():
        pixels = fn(n)
        write_png(dest / f"{name}.png", n, n, pixels)
        print("wrote", name)
    bg, bw, bh = draw_background()
    write_png(dest / "Background.png", bw, bh, bg)
    print("wrote Background")


if __name__ == "__main__":
    main()
