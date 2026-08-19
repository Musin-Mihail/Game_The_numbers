"""Постпроцесс PNG: альфа, палитра UiTheme, фиксированные размеры."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "graphics"))
import style as S

try:
    from PIL import Image
except ImportError as exc:  # pragma: no cover
    raise SystemExit("Install Pillow: pip install Pillow") from exc


def _corners_rgba(im: Image.Image, inset: int = 4) -> tuple[int, int, int, int]:
    px = im.convert("RGBA")
    w, h = px.size
    samples = [
        px.getpixel((inset, inset)),
        px.getpixel((w - 1 - inset, inset)),
        px.getpixel((inset, h - 1 - inset)),
        px.getpixel((w - 1 - inset, h - 1 - inset)),
    ]
    r = sum(p[0] for p in samples) // 4
    g = sum(p[1] for p in samples) // 4
    b = sum(p[2] for p in samples) // 4
    a = sum(p[3] for p in samples) // 4
    return r, g, b, a


def _color_dist(a: tuple[int, ...], b: tuple[int, ...]) -> float:
    return ((a[0] - b[0]) ** 2 + (a[1] - b[1]) ** 2 + (a[2] - b[2]) ** 2) ** 0.5


def _bbox_from_alpha(im: Image.Image, threshold: int = 16) -> tuple[int, int, int, int] | None:
    alpha = im.getchannel("A")
    mask = alpha.point(lambda v: 255 if v >= threshold else 0)
    return mask.getbbox()


def _fit_square(im: Image.Image, size: int, pad_ratio: float = 0.14) -> Image.Image:
    bbox = _bbox_from_alpha(im)
    if bbox is None:
        canvas = Image.new("RGBA", (size, size), (0, 0, 0, 0))
        return canvas
    cropped = im.crop(bbox)
    pad = int(round(max(cropped.size) * pad_ratio))
    inner = max(cropped.size) + pad * 2
    square = Image.new("RGBA", (inner, inner), (0, 0, 0, 0))
    ox = (inner - cropped.width) // 2
    oy = (inner - cropped.height) // 2
    square.paste(cropped, (ox, oy), cropped)
    return square.resize((size, size), Image.Resampling.LANCZOS)


def process_icon(im: Image.Image) -> Image.Image:
    rgba = im.convert("RGBA")
    bg = _corners_rgba(rgba)
    pixels = list(rgba.getdata())
    out = []
    for r, g, b, a in pixels:
        dist = _color_dist((r, g, b), bg)
        # фон: низкая дистанция → прозрачный; глиф → белый
        alpha = 0 if dist < 38 else min(255, int((dist - 28) * 4))
        if a < 16:
            alpha = 0
        # тёмный глиф на светлом фоне тоже ок: инвертируем яркость в альфу
        luma = 0.2126 * r + 0.7152 * g + 0.0722 * b
        if dist < 80 and luma < 140:
            alpha = max(alpha, int(255 - luma))
        out.append((255, 255, 255, alpha if alpha > 12 else 0))
    rgba.putdata(out)
    return _fit_square(rgba, S.ICON_SIZE)


def process_cell(im: Image.Image) -> Image.Image:
    rgba = im.convert("RGBA")
    bg = _corners_rgba(rgba)
    pixels = list(rgba.getdata())
    out = []
    cr, cg, cb = S.CELL[0], S.CELL[1], S.CELL[2]
    for r, g, b, a in pixels:
        dist = _color_dist((r, g, b), bg)
        if dist < 28 or a < 8:
            out.append((0, 0, 0, 0))
            continue
        luma = 0.2126 * r + 0.7152 * g + 0.0722 * b
        alpha = 230 if luma < 245 else 0
        if dist > 40:
            alpha = max(alpha, min(255, int(dist * 3)))
        out.append((cr, cg, cb, alpha if alpha > 20 else 0))
    rgba.putdata(out)
    return _fit_square(rgba, S.ICON_SIZE, pad_ratio=0.06)


def process_background(im: Image.Image) -> Image.Image:
    rgb = im.convert("RGB").resize((S.BACKGROUND_WIDTH, S.BACKGROUND_HEIGHT), Image.Resampling.LANCZOS)
    # лёгкий lift к белому, чтобы фон не спорил с плитками
    lift = 0.22
    pixels = [
        (
            int(r + (255 - r) * lift),
            int(g + (255 - g) * lift),
            int(b + (255 - b) * lift),
        )
        for r, g, b in rgb.getdata()
    ]
    rgb.putdata(pixels)
    return rgb.convert("RGBA")


def process(kind: str, im: Image.Image) -> Image.Image:
    if kind == "icon":
        return process_icon(im)
    if kind == "cell":
        return process_cell(im)
    if kind == "background":
        return process_background(im)
    raise ValueError(f"unknown kind: {kind}")
