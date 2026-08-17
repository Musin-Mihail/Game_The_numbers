# Правила для ИИ-агента

Проект Unity WebGL «The Numbers». Цель — править игру **только текстом** (C#, JSON), без инспектора и без правки YAML сцены.

## Делать

- Менять UI в `Assets/Scripts/View/UI/Builder/` (`UiTheme`, `UiIds`, `PlayingFieldUiBuilder`, `UiFactory`, `WidgetFactory`).
- Логику — в `Assets/Scripts/Model`, `Core`, `Gameplay`.
- Строки — `Assets/Resources/Localization/translations.json`.
- Спрайты — `Assets/Resources/...`, загрузка по пути.
- После заметного изменения — строка в `Docs/CHANGELOG.md`.

## Не делать

- Не редактировать `Assets/Scenes/PlayingField.unity` и `.prefab` ради UI.
- Не вешать ссылки через `[SerializeField]` на сценные объекты.
- Не восстанавливать ScriptableObject-события; использовать `GlobalEvents`.
- Не трогать `Assets/PluginYourGames` без явной задачи по SDK.

Подробности: [Docs/UI_BUILDER.md](Docs/UI_BUILDER.md), [Docs/ARCHITECTURE.md](Docs/ARCHITECTURE.md), [Docs/YandexGamesSDK/README.md](Docs/YandexGamesSDK/README.md). Прямая интеграция SDK без PluginYG2: [Docs/YandexGamesSDK/AGENT.md](Docs/YandexGamesSDK/AGENT.md).
