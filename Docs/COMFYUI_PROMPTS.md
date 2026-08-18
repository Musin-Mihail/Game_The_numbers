# Промпты ComfyUI для графики The Numbers

Игра: портрет 1080×1920, белый фон, голубые плитки, чёрный pixel-шрифт, плоские иконки в квадрате.

Картинки класть в `Assets/Resources/Sprites/` (и `Sprites/Lang/` для флагов). UI грузит только `Resources.Load`, сцену и префабы не трогать. См. [UI_BUILDER.md](UI_BUILDER.md).

## Железо: GTX 5090, 24 ГБ

Хватает Flux.1 Dev и SD3.5 Large в fp16 без агрессивного offload.

| Задача        | Модель                     | Latent                                  | Батч | Заметки                          |
| ------------- | -------------------------- | --------------------------------------- | ---- | -------------------------------- |
| Фон меню/поля | Flux.1 Dev fp16            | 1080×1920 (или 768×1344, потом upscale) | 1    | `weight_dtype: default`          |
| Иконки HUD    | Flux.1 Dev или SD3.5 Large | 1024×1024                               | 1–2  | Потом downscale до 256–512       |
| Клетка        | то же                      | 512×512 или 1024×1024                   | 2–4  | Проще держать серию в одном seed |

Не включать `cpu_offload` / sequential offload на этой карте — только замедлят. Если VRAM упрётся в кастомный upscaler + ControlNet сразу: держать батч = 1 и не поднимать Flux выше ~1280 по короткой стороне без TAESD.

Сэмплер для Flux: `euler` + `simple`, 20–28 шагов, CFG 3.5–5. Для Schnell — 4 шага, CFG 1.

## Стиль (общий хвост)

Добавлять к каждому positive:

```
minimalist puzzle game UI, flat design, clean geometric shapes, light blue and white, high contrast, crisp edges, no photorealism, no 3D lighting, no glossy plastic
```

Общий negative:

```
photorealistic, cinematic, 3d render, blender, unreal engine, noisy, blurry, jpeg artifacts, watermark, signature, extra objects, cluttered, drop shadow, bevel, chrome, neon glow, anime, detailed background
```

Текущие HUD-иконки в коде красятся в почти чёрный (`UiTheme.Icon`). Генерировать **белый глиф на прозрачном фоне**, не чёрный на голубом квадрате.

После генерации: RMBG / BiRefNet → PNG с альфой → в Unity Texture Type = Sprite, Alpha Is Transparency.

## Workflow

### Фон

CheckpointLoaderSimple (Flux Dev) → DualCLIPLoader (flux clip-l + t5xxl) → CLIPTextEncode ×2 → EmptySD3LatentImage (W=1080 H=1920) → KSampler → VAEDecode → SaveImage.

Если 1080×1920 нестабильно у Flux, генерировать 768×1344 и прогнать через 2× upscale (LatentUpscale или 4x-AnimeSharp / 4x-UltraSharp).

### Иконка

Тот же граф, latent 1024×1024. После Decode:

SaveImage → (отдельно) RMBG-2.0 или `Image Remove Background` → Save PNG.

В ComfyUI: `easy imageInsetCrop` не нужен, если промпт просит «centered icon, large, padding».

### Pixel-art вариант клетки

Можно Flux + в промпте `pixel art, 32 pixel sprite, limited palette`. Либо отдельный чекпоинт вроде Pixel Art XL (SD1.5): 512², Euler a, 20 steps, CFG 7.

## Промпты по файлам

Путь — то, что ждёт `UiTheme` / `Resources.Load`.

### Фон поля — пока нет отдельного спрайта

Игра сейчас заливает `background` сплошным белым. Если появится спрайт, логично `Sprites/Background`.

Positive:

```
portrait mobile game background, 9:16, very light gray-white paper, subtle faint geometric grid of rounded squares, airy, clean, minimal, no UI, no numbers, no icons, no text, soft even lighting
```

Negative: общий + `letters, digits, buttons, logo, character, photo`.

Размер: 1080×1920. Не пестрить: сетка должна быть едва заметной, иначе спорит с клетками.

### Фон меню

Positive:

```
portrait 9:16 empty menu backdrop, solid off-white, very subtle cool blue gradient at the bottom, flat, no window, no buttons, no text, no logo
```

Можно тем же файлом, что фон поля.

### Клетка — `Sprites/Cell`

Positive:

```
single UI tile, rounded square, light cornflower blue fill #88b3db, flat color, 1:1, centered, no number, no letter, no icon, even fill, slightly rounded corners, matching mobile puzzle game, white margin around the tile
```

Negative: общий + `digit, text, glyph, crack, wood, stone, gem`.

Размер: 1024², в Unity PPU подогнать к клетке 105 px. Альфа вокруг плитки нужна, если углы скруглены.

### HUD: меню — `Sprites/menu`

Positive:

```
white hamburger menu icon, three equal horizontal bars, thick rounded stroke, centered, large, square canvas, plain white symbol only, transparent background, no circle, no square frame, no text
```

### HUD: добавить линии — `Sprites/add`

Positive:

```
white plus sign icon, thick even strokes, perfectly centered, square canvas, flat, transparent background, no circle, no button frame, no text
```

### HUD: отмена — `Sprites/undo`

Positive:

```
white undo icon, open circular arrow curving counterclockwise, thick stroke, centered, square canvas, flat, transparent background, no clock, no number, no button frame
```

В игре поверх иконки рисуется счётчик (цифра). Не впекать число «5» в спрайт.

### HUD: подсказка — `Sprites/hint`

Positive:

```
white lightbulb icon, simple geometric, thick outline, centered, square canvas, flat, transparent background, no rays, no glow, no button frame, no text
```

Если нужен текущий «знак вопроса» вместо лампы — заменить subject на `white question mark, bold geometric sans`.

### Рейтинг — `Sprites/Rating`

Positive:

```
white leaderboard icon, simple bar chart with three bars and a small star above, thick geometric, centered, square canvas, flat, transparent background, no numbers, no button frame
```

### Закрыть — `Sprites/Close`

Positive:

```
white X close icon, two thick diagonal bars crossing, centered, square canvas, flat, transparent background, no circle, no octagon, no text
```

### Галочка — `Sprites/CheckBox`

Positive:

```
white check mark, thick geometric tick, centered, square canvas, flat, transparent background, no box, no border, no text
```

Игра рисует галочку **внутри** квадрата тоггла. Нужна только галка.

### Кнопка меню (опционально)

Сейчас кнопки — цветной квад + TMP. Если делать спрайт:

```
wide rounded rectangle UI button, light cornflower blue #88b3db, flat, no text, no icon, 4:1 aspect, even fill, slightly rounded corners, white canvas around
```

Latent примерно 1024×256. Текст по-прежнему из локализации, не из картинки.

### Игрок / аватар — `Sprites/player`

Positive:

```
white simple person silhouette icon, geometric head and shoulders, centered, square canvas, flat, transparent background, no face details, no photo
```

### Флаги языков — `Sprites/Lang/EN` … `TR`

Лучше не генерировать нейросетью (кривые флаги). Взять стандартные 4×3 PNG. Имена: `EN`, `ZH`, `ES`, `FR`, `RU`, `DE`, `TR`.

## Импорт в Unity

1. PNG → `Assets/Resources/Sprites/` с теми же именами, что в таблице [UI_BUILDER.md](UI_BUILDER.md).
2. Texture Type: Sprite (2D and UI), Mesh Type: Tight для иконок, Full Rect для фона.
3. Иконки HUD — белые; если получились чёрные, инвертировать до импорта, иначе на белом фоне их не видно даже после tint.
4. Не класть в `Materials/` и не вешать через инспектор сцены.

## Быстрый чеклист качества

- Фон не темнее клеток и без крупных пятен в зоне 3–5 линий сетки.
- Иконка читается при 150×150 (HUD).
- В спрайте нет текста: подписи только из `translations.json`.
- Одна иконка — один глиф, без рамки-кнопки (рамку рисует код, если нужно).
