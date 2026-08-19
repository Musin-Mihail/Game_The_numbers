"""Скачать Flux.1 Dev и текстовые энкодеры в models/ Comfy Desktop.

Запуск из корня репозитория:
    python Tools/comfyui/download_models.py
"""

from __future__ import annotations

import json
import sys
import urllib.request
from pathlib import Path

HERE = Path(__file__).resolve().parent
CONFIG_PATH = HERE / "config.json"


def load_config() -> dict:
    return json.loads(CONFIG_PATH.read_text(encoding="utf-8"))


def models_root(config: dict) -> Path:
    return Path(config["comfyui_root"]) / "models"


def already_ok(path: Path, min_bytes: int = 1_000_000) -> bool:
    return path.is_file() and path.stat().st_size >= min_bytes


def download_hf(dest: Path, repo_id: str, filename: str) -> bool:
    try:
        from huggingface_hub import hf_hub_download
    except ImportError:
        return False

    import shutil

    dest.parent.mkdir(parents=True, exist_ok=True)
    cached = Path(
        hf_hub_download(
            repo_id=repo_id,
            filename=filename,
            resume_download=True,
        )
    )
    if cached.resolve() == dest.resolve():
        return dest.is_file()
    if dest.exists() or dest.is_symlink():
        dest.unlink()
    try:
        dest.symlink_to(cached)
    except OSError:
        shutil.copy2(cached, dest)
    return dest.is_file()


def download_url(url: str, dest: Path) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    tmp = dest.with_suffix(dest.suffix + ".part")
    existing = tmp.stat().st_size if tmp.exists() else 0
    headers = {}
    if existing:
        headers["Range"] = f"bytes={existing}-"
        print(f"resume {dest.name} from {existing} bytes")

    req = urllib.request.Request(url, headers=headers)
    with urllib.request.urlopen(req) as resp, tmp.open("ab" if existing else "wb") as out:
        total = resp.headers.get("Content-Length")
        expected = int(total) + existing if total else None
        copied = existing
        while True:
            chunk = resp.read(1024 * 1024)
            if not chunk:
                break
            out.write(chunk)
            copied += len(chunk)
            if expected:
                pct = 100.0 * copied / expected
                print(f"\r{dest.name}: {copied / 1e9:.2f} / {expected / 1e9:.2f} GB ({pct:.1f}%)", end="", flush=True)
    print()
    tmp.replace(dest)


def main() -> int:
    config = load_config()
    root = models_root(config)
    if not Path(config["comfyui_root"]).is_dir():
        print("ComfyUI root not found:", config["comfyui_root"], file=sys.stderr)
        return 1

    for item in config["models"]:
        dest = root / item["directory"] / item["name"]
        if already_ok(dest):
            print("skip (exists)", dest)
            continue
        print("download", item["name"], "->", dest)
        ok = False
        if item.get("repo_id") and item.get("repo_filename"):
            try:
                ok = download_hf(dest, item["repo_id"], item["repo_filename"])
            except Exception as exc:
                print("huggingface_hub failed:", exc)
        if not already_ok(dest):
            download_url(item["url"], dest)
            ok = already_ok(dest)
        if not ok and not already_ok(dest):
            print("failed", dest, file=sys.stderr)
            return 1
        print("ok", dest, dest.stat().st_size)

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
