"""Очередь Flux.1 Dev в ComfyUI API и запись спрайтов в Resources.

Comfy Desktop должен быть запущен. Из корня репозитория:
    python Tools/comfyui/generate.py
    python Tools/comfyui/generate.py --only menu,Cell,Background
"""

from __future__ import annotations

import argparse
import copy
import io
import json
import sys
import time
import urllib.error
import urllib.parse
import urllib.request
import uuid
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[1]
sys.path.insert(0, str(HERE))
sys.path.insert(0, str(HERE.parent / "graphics"))
import style as S  # noqa: E402
from postprocess import process  # noqa: E402

try:
    from PIL import Image
except ImportError as exc:  # pragma: no cover
    raise SystemExit("Install Pillow: pip install Pillow") from exc


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def http_json(url: str, data: dict | None = None, timeout: float = 30) -> dict:
    body = None
    headers = {"Accept": "application/json"}
    if data is not None:
        body = json.dumps(data).encode("utf-8")
        headers["Content-Type"] = "application/json"
    req = urllib.request.Request(url, data=body, headers=headers)
    with urllib.request.urlopen(req, timeout=timeout) as resp:
        raw = resp.read()
        return json.loads(raw.decode("utf-8") or "{}")


def http_bytes(url: str, timeout: float = 60) -> bytes:
    with urllib.request.urlopen(url, timeout=timeout) as resp:
        return resp.read()


def find_api(bases: list[str]) -> str:
    last_error = None
    for base in bases:
        try:
            http_json(base.rstrip("/") + "/system_stats", timeout=5)
            return base.rstrip("/")
        except Exception as exc:
            last_error = exc
    raise SystemExit(
        "ComfyUI API не отвечает на "
        + ", ".join(bases)
        + f". Запустите Comfy Desktop и повторите.\nПоследняя ошибка: {last_error}"
    )


def gen_size(kind: str) -> tuple[int, int]:
    if kind == "background":
        return S.GEN_BACKGROUND_WIDTH, S.GEN_BACKGROUND_HEIGHT
    if kind == "cell":
        return S.GEN_CELL_SIZE, S.GEN_CELL_SIZE
    return S.GEN_ICON_SIZE, S.GEN_ICON_SIZE


def build_prompt(subject: str, style_positive: str) -> str:
    return f"{subject}, {style_positive}"


def fill_workflow(template: dict, prompt: str, width: int, height: int, seed: int, prefix: str) -> dict:
    graph = copy.deepcopy(template)
    graph["7"]["inputs"]["text"] = prompt
    graph["8"]["inputs"]["guidance"] = S.GUIDANCE
    graph["10"]["inputs"]["width"] = width
    graph["10"]["inputs"]["height"] = height
    graph["11"]["inputs"]["seed"] = seed
    graph["11"]["inputs"]["steps"] = S.STEPS
    graph["11"]["inputs"]["cfg"] = S.CFG
    graph["11"]["inputs"]["sampler_name"] = S.SAMPLER
    graph["11"]["inputs"]["scheduler"] = S.SCHEDULER
    graph["11"]["inputs"]["denoise"] = S.DENOISE
    graph["13"]["inputs"]["filename_prefix"] = prefix
    return graph


def queue_prompt(api: str, workflow: dict, client_id: str) -> str:
    payload = {"prompt": workflow, "client_id": client_id}
    try:
        result = http_json(api + "/prompt", payload, timeout=60)
    except urllib.error.HTTPError as exc:
        detail = exc.read().decode("utf-8", errors="replace")
        raise SystemExit(f"ComfyUI /prompt {exc.code}: {detail}") from exc
    if "error" in result:
        raise SystemExit(f"ComfyUI error: {result}")
    prompt_id = result.get("prompt_id")
    if not prompt_id:
        raise SystemExit(f"No prompt_id in {result}")
    return prompt_id


def wait_history(api: str, prompt_id: str, timeout: float = 600) -> dict:
    started = time.time()
    while time.time() - started < timeout:
        hist = http_json(api + "/history/" + prompt_id, timeout=30)
        if prompt_id in hist:
            entry = hist[prompt_id]
            status = entry.get("status") or {}
            if status.get("status_str") == "error" or entry.get("status_str") == "error":
                raise SystemExit(f"ComfyUI job failed: {json.dumps(entry.get('status'), ensure_ascii=False)}")
            if entry.get("outputs"):
                return entry
        time.sleep(1.5)
    raise SystemExit(f"Timeout waiting for {prompt_id}")


def first_image_meta(history: dict) -> dict:
    outputs = history.get("outputs") or {}
    for node in outputs.values():
        images = node.get("images") or []
        if images:
            return images[0]
    raise SystemExit(f"No images in history outputs: {list(outputs)}")


def fetch_image(api: str, meta: dict) -> Image.Image:
    query = urllib.parse.urlencode(
        {
            "filename": meta["filename"],
            "subfolder": meta.get("subfolder") or "",
            "type": meta.get("type") or "output",
        }
    )
    data = http_bytes(api + "/view?" + query)
    return Image.open(io.BytesIO(data))


def parse_only(raw: str | None, assets: list[dict]) -> list[dict]:
    if not raw:
        return assets
    wanted = {name.strip() for name in raw.split(",") if name.strip()}
    selected = [a for a in assets if a["name"] in wanted]
    missing = wanted - {a["name"] for a in selected}
    if missing:
        raise SystemExit("Unknown --only names: " + ", ".join(sorted(missing)))
    return selected


def main() -> int:
    parser = argparse.ArgumentParser(description="Generate The Numbers sprites via ComfyUI Flux.1 Dev")
    parser.add_argument("--only", help="Comma-separated asset names, e.g. menu,Cell,Background")
    parser.add_argument("--seed", type=int, default=S.SEED)
    args = parser.parse_args()

    config = load_json(HERE / "config.json")
    manifest = load_json(HERE / "assets.json")
    workflow_path = HERE / config["workflow"]
    template = load_json(workflow_path)
    assets = parse_only(args.only, manifest["assets"])
    out_dir = REPO / config["output_dir"]
    out_dir.mkdir(parents=True, exist_ok=True)

    api = find_api(config["api_bases"])
    print("API", api)
    client_id = config.get("client_id") or str(uuid.uuid4())
    style_positive = manifest.get("style_positive") or S.STYLE_POSITIVE

    for index, asset in enumerate(assets):
        name = asset["name"]
        kind = asset["kind"]
        width, height = gen_size(kind)
        seed = args.seed + index
        prompt = build_prompt(asset["prompt"], style_positive)
        prefix = f"the_numbers/{name}"
        graph = fill_workflow(template, prompt, width, height, seed, prefix)
        print(f"queue {name} {width}x{height} seed={seed}")
        prompt_id = queue_prompt(api, graph, client_id)
        history = wait_history(api, prompt_id)
        image = fetch_image(api, first_image_meta(history))
        processed = process(kind, image)
        dest = out_dir / f"{name}.png"
        processed.save(dest, "PNG")
        print("wrote", dest)

    print("done", len(assets), "sprites")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
