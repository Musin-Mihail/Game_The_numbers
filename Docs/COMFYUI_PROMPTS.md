# Промпты ComfyUI для графики The Numbers

Игра: портрет 1080×1920, белый фон, голубые плитки, чёрный pixel-шрифт, плоские иконки в квадрате.

Картинки класть в `Assets/Resources/Sprites/` (и `Sprites/Lang/` для флагов). UI грузит только `Resources.Load`, сцену и префабы не трогать. См. [UI_BUILDER.md](UI_BUILDER.md). `.meta` не перезаписывать.

**Основной путь:** Flux.1 Dev через API Comfy Desktop. Промпты, seed и размеры — [`Tools/graphics/style.py`](../Tools/graphics/style.py) и [`Tools/comfyui/assets.json`](../Tools/comfyui/assets.json). Python-геометрия [`Tools/graphics/generate_sprites.py`](../Tools/graphics/generate_sprites.py) — только запасной вариант без GPU.

## Запуск

Comfy Desktop должен быть открыт (этот репозиторий ждёт `http://127.0.0.1:8188`, иначе `:8000`). Корень инсталла задан в [`Tools/comfyui/config.json`](../Tools/comfyui/config.json).

```
pip install -r Tools/comfyui/requirements.txt
python Tools/comfyui/download_models.py
python Tools/comfyui/generate.py
python Tools/comfyui/generate.py --only menu,Cell,Background
```

Скачивание (Comfy-Org, без gated BFL): `flux1-dev.safetensors` (~23 ГБ), `t5xxl_fp16.safetensors`, `clip_l.safetensors`, `ae.safetensors` → `ComfyUI/models/{diffusion_models,text_encoders,vae}/`.

Workflow API: [`Tools/comfyui/workflows/flux_txt2img_api.json`](../Tools/comfyui/workflows/flux_txt2img_api.json) — UNETLoader + DualCLIPLoader + VAELoader + EmptySD3LatentImage + CLIPTextEncode + FluxGuidance + ConditioningZeroOut + KSampler + VAEDecode. После Decode клиент режет фон, красит клетку в `#88B3DB` и кладёт PNG в Resources.

## Железо: GTX 5090, 24 ГБ

| Задача        | Модель          | Latent                | Батч | Заметки                                 |
| ------------- | --------------- | --------------------- | ---- | --------------------------------------- |
| Фон меню/поля | Flux.1 Dev fp16 | 768×1344 → 1080×1920  | 1    | `weight_dtype: default`                 |
| Иконки HUD    | Flux.1 Dev fp16 | 1024×1024 → 256       | 1    | белый глиф, tint в игре                 |
| Клетка        | то же           | 1024×1024 → 256       | 1    | перекраска в `#88B3DB` после генерации  |

Не включать `cpu_offload`. Сэмплер: `euler` + `simple`, 20 шагов, CFG 1, Flux guidance 3.5.

## Стиль

Общий хвост и negative живут в `style.py` / `assets.json`. HUD-иконки в коде красятся в `UiTheme.Icon` — нужен **белый глиф на прозрачном фоне**. В спрайт не впекать цифры и подписи: текст только из `translations.json`. Флаги `Sprites/Lang/EN` … `DE` нейросетью не генерировать.

## Чеклист качества

- Фон не темнее клеток и без крупных пятен в зоне 3–5 линий сетки.
- Иконка читается при 150×150 (HUD).
- Клетка без цифры; число рисует TMP и должно сидеть по центру плитки.
- Одна иконка — один глиф, без рамки-кнопки.
