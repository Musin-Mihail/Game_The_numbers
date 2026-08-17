# Прямая интеграция Yandex Games SDK (без PluginYG2)

Инструкция для ИИ-агента. Официального гайда «Unity без плагина» у Яндекса **нет**. Раздел `sdk/unity/` — это видеокурс **Plugin Your Games 2.0** (тот же PluginYG2). Нативный контракт платформы — JavaScript SDK.

Снимок документации: **2026-08-18**, источник `https://yandex.ru/dev/games/doc/ru/`. Индекс: [llms.txt](llms.txt). Полный дамп сайта (все движки): `https://yandex.ru/dev/games/doc/ru/llms-full.txt` — не копировать в репозиторий, читать точечно по индексу.

## Что читать (порядок)

1. Этот файл — карта задачи и мост Unity WebGL.
2. [`types/ysdk.d.ts`](types/ysdk.d.ts) — полный контракт API (`@types/ysdk@1.2.0`). Сигнатуры методов брать отсюда.
3. [`sdk/sdk-about.connect.md`](sdk/sdk-about.connect.md) — как подключить `/sdk.js` (в markdown-снимке теги `<script>` вырезаны санитизацией).
4. Страницы `sdk/` по фичам игры (таблица ниже).
5. [`requirements/sdk-methods.md`](requirements/sdk-methods.md) — пункт 1.19 (модерация: лоадер, Game Ready, GameplayAPI).
6. [`concepts/local-launch.md`](concepts/local-launch.md) и [`console/debug-panel.md`](console/debug-panel.md) — проверка без плагина.

Не читать для реализации: `unity/*.md`, Mobile Ads Unity, Cocos, Construct 3, Defold.

Онлайн-страницы (если локальный снимок устарел):

- https://yandex.ru/dev/games/doc/ru/sdk/sdk-about.md
- https://yandex.ru/dev/games/doc/ru/sdk.md
- https://yandex.ru/dev/games/doc/ru/llms.txt

## Что уже есть в билде

Шаблон `Assets/WebGLTemplates/YandexGames/index.html` **уже** подключает нативный SDK:

```html
<script src="/sdk.js"></script>
```

и вызывает `ysdk = await YaGames.init()`. PluginYG2 — C#/JS-обёртка вокруг этого. Прямая интеграция = оставить `/sdk.js` + `YaGames.init()`, выкинуть `YG2.*` и `Assets/PluginYourGames`.

Для хостинга на серверах Яндекса путь **только** `/sdk.js` (относительный). Абсолютный `https://sdk.games.s3.yandex.net/sdk.js` — только свой домен / iframe. Скрипт должен загрузиться **до** `YaGames.init()`.

## Архитектура замены

Игра уже прячет платформу за интерфейсы. Не тащить `ysdk` в UI. Слой:

```
View / Gameplay  →  Interfaces (ISaveLoadService, ILeaderboardService, IPlatformServices)
                 →  Core/Platform/*  →  тонкий C# фасад  →  .jslib  →  window.ysdk
```

Unity WebGL (это не документация Яндекса, а движок):

- JS → C#: `unityInstance.SendMessage('GameObjectName', 'MethodName', stringOrNumber)`.
- C# → JS: файл `*.jslib` с `mergeInto(LibraryManager.library, { Foo: function () { ... } })` и в C# `[DllImport("__Internal")] private static extern void Foo();`.
- В Editor заглушки: `#if UNITY_WEBGL && !UNITY_EDITOR`.
- Инициализацию `YaGames.init()` держать в `index.html` шаблона (как сейчас), не внутри каждого `.jslib`.
- Платежи на клиенте: `YaGames.init()` без `{ signed: true }` (сервера своей игры нет).

Обязательные вызовы модерации (п. 1.19):

| Когда                                                    | Метод                                                 |
| -------------------------------------------------------- | ----------------------------------------------------- |
| После `YaGames.init()`, скрипт `/sdk.js` в `<head>`      | лоадер `IT` на debug-панели                           |
| Игра интерактивна (меню/поле готовы, нет своего лоадера) | `ysdk.features.LoadingAPI.ready()`                    |
| Игрок играет / пауза меню-реклама-магазин                | `ysdk.features.GameplayAPI.start()` / `stop()`        |
| Реклама, покупки, смена вкладки                          | `ysdk.on('game_api_pause' \| 'game_api_resume', ...)` |

`GameplayAPI` и подписка на pause/resume **опциональны**, но если вызывать — строго по `sdk-game-events.md` и `sdk-events.md`. Сейчас это делает PluginYG2 в шаблоне; при удалении плагина перенести в свой шаблон/фасад.

## Карта YG2 → ysdk (эта игра)

| Сейчас (PluginYG2)                            | Нативный SDK                                                                       | Документация    |
| --------------------------------------------- | ---------------------------------------------------------------------------------- | --------------- |
| `YG2.onGetSDKData`, `YG2.isSDKEnabled`        | `YaGames.init()` завершился, `ysdk` есть                                           | sdk-about       |
| `YG2.lang`                                    | `ysdk.environment.i18n.lang`                                                       | sdk-environment |
| `YG2.saves`, `YG2.SaveProgress()`             | `player.setData(data, flush)`, `player.getData()`                                  | sdk-player      |
| `YG2.onDefaultSaves`                          | пустой объект из `getData()` (новый игрок)                                         | sdk-player      |
| `YG2.player.auth`                             | `player.isAuthorized()` (`getMode()` устарел)                                      | sdk-player      |
| `YG2.player.id`                               | `player.getUniqueID()`                                                             | sdk-player      |
| `YG2.SetLeaderboard(name, score)`             | `ysdk.leaderboards.setScore(name, score)`                                          | sdk-leaderboard |
| `YG2.GetLeaderboard(...)`, `onGetLeaderboard` | `ysdk.leaderboards.getEntries(name, { quantityTop, quantityAround, includeUser })` | sdk-leaderboard |
| `YG2.InterstitialAdvShow()`                   | `ysdk.adv.showFullscreenAdv({ callbacks })`                                        | sdk-adv         |
| `YG2.RewardedAdvShow(id)`, `onRewardAdv`      | `ysdk.adv.showRewardedVideo({ callbacks: { onRewarded } })`                        | sdk-adv         |
| `YG2.BuyPayments(id)`                         | `(await ysdk.getPayments()).purchase({ id })`                                      | sdk-purchases   |
| `YG2.PurchaseByID`, `ConsumePurchaseByID`     | `payments.getPurchases()` + `payments.consumePurchase(token)`                      | sdk-purchases   |
| `YG2.onPurchaseSuccess` / `Failed`            | callbacks/promise `purchase()`                                                     | sdk-purchases   |
| авто Game Ready (InfoYG AutoGRA)              | `ysdk.features.LoadingAPI.ready()` из C# после готовности UI                       | sdk-game-events |
| пауза в шаблоне                               | `ysdk.on('game_api_pause'/'game_api_resume')`                                      | sdk-events      |

Сохранения: PluginYG2 сериализует C#-класс `SavesYG` в JSON и кладёт в облако. Напрямую это `player.setData({ ...поля })`. Формат JSON можно оставить тем же, чтобы не потерять прогресс игроков. Лимит и ключи — в `sdk-player.md` (`setData`, `getData`). `flush: true` — сразу на сервер.

Лидерборды: не использовать устаревший `ysdk.getLeaderboards()`. Только `ysdk.leaderboards.*`.

Покупки: после загрузки вызывать `getPurchases()` и консумировать необработанные (иначе модерация 1.13). Каталог — `payments.getCatalog()`, цена/валюта с продукта SDK, не хардкод.

Реклама: не вызывать fullscreen по `setInterval`. Rewarded — без лимита частоты платформой. Звук глушить на `game_api_pause`.

Мультиплеер, sticky-баннер, review, shortcut, remote config — игра сейчас не использует; не внедрять «на всякий случай».

Файлы игры с прямым `YG2` (заменить, не трогая сцену YAML):

- `Assets/Scripts/Core/GameBootstrap.cs`
- `Assets/Scripts/Core/GameManager.cs`
- `Assets/Scripts/Core/GameController.cs`
- `Assets/Scripts/Core/LeaderboardUpdater.cs`
- `Assets/Scripts/Core/Platform/*`
- `Assets/Scripts/Core/Handlers/*`
- `Assets/Scripts/Core/Shop/ShopManager.cs`
- `Assets/Scripts/View/UI/*` (StatisticsView, LeaderboardView, OptionsWindowManager, ButtonAnimator, StatisticsWindowManager)

После замены удалить `Assets/PluginYourGames` целиком. Шаблон `WebGLTemplates/YandexGames` переписать: убрать `YG2Instance` / `SendMessage('YG2Instance', ...)`, оставить `/sdk.js`, `YaGames.init()`, LoadingAPI, GameplayAPI, pause/resume.

В Editor без WebGL — локальные заглушки (сейчас плагин подменяет SDK). Для проверки настоящего SDK: [`concepts/local-launch.md`](concepts/local-launch.md) (`@yandex-games/sdk-dev-proxy`) и debug-панель `?debug-mode=16`. Индикатор лоадера должен быть `IT`, не `IF`.

## Чего нет в документации Яндекса

Яндекс не описывает `.jslib`, `DllImport`, WebGL Template, `SendMessage`. Это документация Unity:

- [WebGL: interacting with browser scripting](https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html)

Готового C# SDK «от Яндекса без плагина» нет. Писать тонкую обёртку под интерфейсы проекта, не клонировать PluginYG2.
