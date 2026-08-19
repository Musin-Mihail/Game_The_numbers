"""Палитра, размеры и стилевой хвост графики The Numbers. Совпадает с UiTheme."""

# Выход: Assets/Resources/Sprites/
ICON_SIZE = 256
BACKGROUND_WIDTH = 1080
BACKGROUND_HEIGHT = 1920
GRID_COLUMNS = 10

# Генерация Flux → потом downscale/upscale до ICON_SIZE / BACKGROUND_*
GEN_ICON_SIZE = 1024
GEN_CELL_SIZE = 1024
GEN_BACKGROUND_WIDTH = 768
GEN_BACKGROUND_HEIGHT = 1344

# UiTheme.CanvasBackground / CellOriginal / Icon
WHITE = (255, 255, 255, 255)
CELL = (136, 179, 219, 255)  # 0.533, 0.702, 0.859
CELL_FILL = (136, 179, 219, 230)
CELL_HEX = "#88B3DB"
ICON_RGB = (255, 255, 255)  # белый глиф, tint в игре
BACKGROUND = (255, 255, 255, 255)
GRID_LINE = (200, 214, 230, 40)

# Flux.1 Dev (официальный blueprint Comfy Desktop)
SEED = 4242
STEPS = 20
CFG = 1.0
GUIDANCE = 3.5
SAMPLER = "euler"
SCHEDULER = "simple"
DENOISE = 1.0
WEIGHT_DTYPE = "default"

STYLE_POSITIVE = (
    "minimalist puzzle game UI, flat design, clean geometric shapes, "
    "light blue and white, high contrast, crisp edges, no photorealism, "
    "no 3D lighting, no glossy plastic"
)

STYLE_NEGATIVE = (
    "photorealistic, cinematic, 3d render, blender, unreal engine, noisy, "
    "blurry, jpeg artifacts, watermark, signature, extra objects, cluttered, "
    "drop shadow, bevel, chrome, neon glow, anime, detailed background, "
    "letters, digits, text, logo"
)

SPRITE_NAMES = (
    "menu",
    "add",
    "undo",
    "hint",
    "Rating",
    "Close",
    "CheckBox",
    "gear",
    "play",
    "player",
    "Cell",
    "Background",
)
