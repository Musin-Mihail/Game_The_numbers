# Журнал изменений

Формат: дата — что и зачем. Писать сюда каждый заметный шаг рефакторинга «проект для агента».

## 2026-08-19

- Языки: en, ru, es, fr, de. Китайский и турецкий убраны (SDK без них падает на en). CJK-шрифт удалён. Пересборка pixel-атласа: меню Unity «The Numbers / Rebuild Pixel Font».
- Меню: шестерёнка слева сверху (опции), зелёный треугольник по центру (играть). Спрайты `Sprites/gear` и `Sprites/play`.
- Страница «Правила» и кнопка в меню удалены. Те же правила показываются в обучении: текст под сеткой и подсветка пар. «Начать заново» и «Сброс данных» сбрасывают обучение.
- Промпты для фона ComfyUI: `Docs/COMFYUI_PROMPTS.md`.

## 2026-08-18

- PluginYG2 заменён тонким мостом `YandexGamesSdk` → `ysdk`. Облачный JSON в формате `{ saves: [json] }`. EditMode-тесты в `Assets/Tests/Editor/`. Курс `Docs/YandexGamesSDK/unity/` удалён; остальные страницы `sdk/` оставлены.
- HUD-иконки (`menu`, `add`, `undo`, `hint`, `Rating`) красятся в `UiTheme.Icon` (почти чёрный): спрайты белые, на светлом фоне их не было видно.
- Окна меню, правил, опций и рейтинга с непрозрачным белым фоном — сетка больше не просвечивает сквозь оверлеи.
- Всплывающие очки: хост `Score` в той же системе координат, что и ячейки (pivot сверху слева); цифры рисуются поверх сетки.
- В окне выбора языка — названия на родном языке (English, 中文, …). Спрайт с кодом (EN/RU) только на кнопке в углу.
- Вместо `GetBuiltinResource("UI/Skin/UISprite.psd")` — кэшированный белый спрайт: Image без спрайта не рисуется, а builtin-ресурс в Unity 6 отсутствует.
- Модели (`GridModel`, `StatisticsModel`, …) регистрируются до `PlayingFieldUiBuilder`, чтобы `StatisticsView.OnEnable` не искал сервис раньше времени.
- HUD-подписи без глифов LiberationSans: `M` / `U` вместо `☰` / `↩`.
- Фон главного меню непрозрачный (`MenuOverlay`), чтобы поле не просвечивало.
- HUD-кнопки вешаются после сборки иерархии: `ActionCountersView.Awake` больше не ищет несуществующие кнопки.
- Спрайты и шрифты перенесены в `Assets/Resources/Sprites` и `Fonts`. `UiTheme` грузит только `Resources.Load`.

## 2026-08-17

- Интерфейс собирается кодом при старте (`PlayingFieldUiBuilder`). Агент меняет UI в C#, без Unity Editor.
- Старый Canvas / `Game/UI` / `Game/GAMEPLAY` в сцене выключены и уничтожаются при запуске.
- Ячейка, всплывающие очки и строка лидерборда создаются `WidgetFactory` (имена детей совпадают с BindUI).
- Клик по ячейке вешается в коде; кнопка Rating вызывает `GlobalEvents.OnShowStatistics`.
- `LocalizableText.Bind`, флаги языков через `Resources` или цветные заглушки.
- Документы: `Docs/`, корневой `AGENTS.md`.
- HUD-иконки и флаги читаются из `Resources/Sprites` (карта имён как в `Assets/Materials/Sprites`). Шрифт — `Resources/Fonts/light_pixel-7_main`, иначе TMP default.
- Unity 6: `FindAnyObjectByType` / `FindObjectsByType` без SortMode; TMP `textWrappingMode`; исправлен вызов `AddImage` для флагов языков.
