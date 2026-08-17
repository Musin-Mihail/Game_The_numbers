# Архитектура

Логическая игра-головоломка для Яндекс.Игр (Unity WebGL). Пары: одинаковые числа или сумма 10, без активных клеток между ними.

## Слои

- **Model** — `GridModel`, `StatisticsModel`, `ActionCountersModel`, `CellData`.
- **View** — `View/Grid`, `View/UI`. Поиск виджетов по именам (`UiIds`), не по ссылкам в инспекторе.
- **Core** — `GameBootstrap`, `GameController`, хендлеры, платформа, undo, магазин.
- **Gameplay** — `MatchValidator` через `IGridDataProvider`.

События — статический класс `Core.Events.GlobalEvents`, не ScriptableObject.

Зависимости — `ServiceProvider` (service locator). Модели и сервисы создаются в `GameBootstrap.Awake`.

## Старт сцены

1. `GameBootstrap` (`DefaultExecutionOrder -100`) регистрирует `LocalizationManager`.
2. `PlayingFieldUiBuilder.ReplaceSceneUi()` удаляет старый Canvas / `Game/UI` / `Game/GAMEPLAY` и собирает интерфейс.
3. Bootstrap находит компоненты по типу и внедряет зависимости.

На сцене должны остаться: камера, свет, EventSystem, `Game/SYSTEMS` (bootstrap, GameManager, LeaderboardUpdater, AdTimerManager). YG2 создаётся плагином в DontDestroyOnLoad.

## Сохранения и платформа

`YandexSaveLoadService`, `YandexLeaderboardService`, `YandexPlatformService` реализуют интерфейсы в `Assets/Scripts/Interfaces`.
