# UI из кода

Источник правды интерфейса — C# в `Assets/Scripts/View/UI/Builder/`, не `PlayingField.unity`.

## Файлы

| Файл                       | Роль                                      |
| -------------------------- | ----------------------------------------- |
| `UiIds.cs`                 | Имена объектов, которые ищет BindUI       |
| `UiTheme.cs`               | Цвета, размеры, шрифт, спрайты            |
| `UiFactory.cs`             | Примитивы (panel, button, text)           |
| `WidgetFactory.cs`         | Ячейка, floating score, строка лидерборда |
| `PlayingFieldUiBuilder.cs` | Сборка экранов при старте                 |

## Как добавить кнопку

1. Имя — константа в `UiIds`, если его будет искать View.
2. Создать объект в нужном методе `PlayingFieldUiBuilder`.
3. Подписать клик в существующем `BindUI` или через `GlobalEvents`.
4. Текст — ключ в `Assets/Resources/Localization/translations.json` и `LocalizableText.Bind(key)`.

## Спрайты и шрифты

Рантайм грузит только `Resources.Load` из `Assets/Resources/`. Новые картинки класть сразу туда, не в `Materials`.

| Путь для `Resources.Load`                                                      | Файл                                       |
| ------------------------------------------------------------------------------ | ------------------------------------------ |
| `Sprites/Cell`                                                                 | `Resources/Sprites/Cell.png`               |
| `Sprites/menu`, `add`, `undo`, `hint`, `Rating`, `player`, `Close`, `CheckBox` | `Resources/Sprites/*.png`                  |
| `Sprites/Lang/EN` … `TR`                                                       | `Resources/Sprites/Lang/EN.png`            |
| `Fonts/light_pixel-7_main`                                                     | `Resources/Fonts/light_pixel-7_main.asset` |

`Image` без спрайта не рисуется — если файл не найден, `UiTheme` подставляет белый квад. Не вызывать `Resources.GetBuiltinResource`.

Не редактировать `.unity` и `.prefab` для UI. Префабы в `Resources/Prefabs` больше не используются рантаймом.

## Имена, которые нельзя ломать без правки View

`ScrollView`, `Content` (внутри `Viewport`), `Container` (внутри `GameSpace`), `TutorialCaption` (текст обучения в `GameSpace`), `Menu` (окно — прямой ребёнок Canvas; кнопка HUD — внутри `Game/Buttons`), `Txt_Score`, `Txt_Record`, `Obj_UndoCount`, `Leaderboard`, `DisabledCounters`, `Btn_Yes` / `Btn_No`, кнопки языков `EN`/`RU`/…
