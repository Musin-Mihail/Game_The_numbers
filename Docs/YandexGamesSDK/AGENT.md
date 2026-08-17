# Yandex Games SDK в этой игре

Официального гайда «Unity без плагина» у Яндекса **нет**. Нативный контракт — JavaScript SDK. Снимок: **2026-08-18**, `https://yandex.ru/dev/games/doc/ru/`. Индекс: [llms.txt](llms.txt).

Код моста:

- C#: `Assets/Scripts/YandexGames/` (`YandexGamesSdk`, `CloudSaveCodec`, `GameSaveData`, `InterstitialAdPolicy`)
- JS: `Assets/Plugins/YandexGames/YandexGames.jslib`
- Шаблон: `Assets/WebGLTemplates/YandexGames/index.html` (`<script src="/sdk.js">`, `ysdk = await YaGames.init()`)

Не клонировать PluginYG2. Не внедрять review / shortcut / remote config / sticky / метрику, если игра это не вызывает. Страницы этих API в [`sdk/`](sdk/) оставлены для других проектов.

## Что читать

1. Этот файл.
2. [`types/ysdk.d.ts`](types/ysdk.d.ts)
3. [`sdk/sdk-about.connect.md`](sdk/sdk-about.connect.md)
4. Страницы `sdk/` по фиче
5. [`requirements/sdk-methods.md`](requirements/sdk-methods.md)
6. [`concepts/local-launch.md`](concepts/local-launch.md), [`console/debug-panel.md`](console/debug-panel.md)

Онлайн, если снимок устарел: https://yandex.ru/dev/games/doc/ru/sdk.md

## Слой

```
View / Gameplay  →  Interfaces  →  Core/Platform  →  YandexGamesSdk  →  .jslib  →  window.ysdk
```

- JS → C#: `SendMessage('YandexGamesSdk', method, arg)`
- C# → JS: `[DllImport("__Internal")]` в WebGL
- Editor: заглушки (`#if UNITY_WEBGL && !UNITY_EDITOR` выключен), сохранения в JSON на диск
- `YaGames.init()` без `{ signed: true }` (своего платёжного сервера нет)

Обязательные вызовы модерации (п. 1.19):

| Когда                                                    | Метод                                          |
| -------------------------------------------------------- | ---------------------------------------------- |
| Скрипт `/sdk.js` в `<head>`, затем `YaGames.init()`      | лоадер `IT` на debug-панели                    |
| Свой лоадер скрыт, UI готов                              | `ysdk.features.LoadingAPI.ready()`             |
| Игрок играет / пауза меню-реклама                        | `GameplayAPI.start()` / `stop()`               |
| Реклама, покупки, смена вкладки                          | `ysdk.on('game_api_pause' \| 'game_api_resume')` |

## Соответствие фасада и ysdk

| `YandexGamesSdk`                         | Нативный SDK                                                                       | Документация    |
| ---------------------------------------- | ---------------------------------------------------------------------------------- | --------------- |
| `Ready`, `IsReady`                       | `YaGames.init()` завершился, `ysdk` есть                                           | sdk-about       |
| `Lang`                                   | `ysdk.environment.i18n.lang`                                                       | sdk-environment |
| `Saves`, `SaveProgress()`                | `player.setData({ saves: [json] }, flush)`, `player.getData(['saves'])`            | sdk-player      |
| `DefaultSaves`                           | пустой объект из `getData()` (новый игрок)                                         | sdk-player      |
| `IsAuthorized`                           | `player.isAuthorized()`                                                            | sdk-player      |
| `PlayerId`                               | `player.getUniqueID()`                                                             | sdk-player      |
| `SetLeaderboardScore`                    | `ysdk.leaderboards.setScore`                                                       | sdk-leaderboard |
| `RequestLeaderboard`                     | `ysdk.leaderboards.getEntries`                                                     | sdk-leaderboard |
| `ShowInterstitial`                       | `ysdk.adv.showFullscreenAdv`                                                       | sdk-adv         |
| `ShowRewarded`                           | `ysdk.adv.showRewardedVideo`                                                       | sdk-adv         |
| `Purchase` / `GetProduct` / `Consume`    | `getPayments()`, `purchase`, `getCatalog`, `getPurchases`, `consumePurchase`       | sdk-purchases   |
| `NotifyGameReady`                        | `ysdk.features.LoadingAPI.ready()` после скрытия своего лоадера                    | sdk-game-events |
| `OnPlatformPause` / `Resume`             | `ysdk.on('game_api_pause'/'game_api_resume')`                                      | sdk-events      |

Имена полей JSON `GameSaveData` совпадают со старым `SavesYG`. Облачная обёртка `{ saves: [jsonString] }` обязательна, иначе прогресс игроков с PluginYG2 пропадёт. Лимит — в `sdk-player.md`. `flush: true`.

Лидерборды: только `ysdk.leaderboards.*`, не устаревший `getLeaderboards()`.

Покупки: после загрузки `getPurchases()` и consume необработанных (модерация 1.13). Цена с каталога SDK.

Реклама: не вызывать fullscreen по `setInterval`. Кулдаун в `InterstitialAdPolicy` (180 с сессии, 60 с между успехами). Звук глушить на `game_api_pause`.

Проверка настоящего SDK: local-launch и `?debug-mode=16`. Лоадер должен быть `IT`, не `IF`.

EditMode-тесты: `Assets/Tests/Editor/`. Прогон:

```
Unity.exe -batchmode -nographics -projectPath <repo> -runTests -testPlatform EditMode -testResults TestResults-EditMode.xml
```

Unity WebGL scripting: https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
