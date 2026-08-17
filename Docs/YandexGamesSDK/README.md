# Yandex Games SDK (снимок документации)

Снимок официальной документации [Яндекс Игр](https://yandex.ru/dev/games/doc/ru/sdk.md) от **2026-08-18**.
Язык: русский. Источник markdown: `https://yandex.ru/dev/games/doc/ru/...`.

Индекс всех страниц платформы: [llms.txt](llms.txt). Онлайн: [llms.txt](https://yandex.ru/dev/games/doc/ru/llms.txt).

Код этой игры: тонкий мост `Assets/Scripts/YandexGames/` + `Assets/Plugins/YandexGames/YandexGames.jslib`. Карта: [AGENT.md](AGENT.md).

## Какие из трёх ссылок нужны этой игре

Проект — Unity **WebGL** для каталога Яндекс Игр, не нативное мобильное приложение.

| Ссылка                                                                    | Что это                                       | Нужно ли                                                                          |
| ------------------------------------------------------------------------- | --------------------------------------------- | --------------------------------------------------------------------------------- |
| [Yandex Games SDK](https://yandex.ru/dev/games/doc/ru/sdk.md)             | Платформенный SDK: `YaGames.init()`, `ysdk.*` | **Да.** Локально: [`sdk/`](sdk/), контракт: [`types/ysdk.d.ts`](types/ysdk.d.ts). |
| [Quick start](https://yandex.ru/dev/games/doc/ru/concepts/quick-start.md) | Регистрация, черновик, модерация              | **Да, как чеклист публикации.**                                                   |
| [Yandex Mobile Ads Unity](https://ads.yandex.com/helpcenter/en/dev/unity) | Mobile Ads SDK 8 для Android/iOS              | **Нет.** Другой продукт.                                                          |

Эта игра вызывает: player (облако), environment (язык), adv (fullscreen/rewarded), leaderboards, purchases, LoadingAPI/GameplayAPI, pause/resume.

Остальные страницы [`sdk/`](sdk/) (review, shortcut, remote config, server time, other-games, …) оставлены как справочник для других WebGL-игр, чтобы не качать снимок заново. В код этой игры их не внедрять.

В исходном markdown теги `<script>` вырезаны санитизацией. Восстановленные примеры: [`sdk/sdk-about.connect.md`](sdk/sdk-about.connect.md). Для хостинга Яндекса: `/sdk.js` **до** `YaGames.init()`.

## С чего читать агенту

1. [AGENT.md](AGENT.md) — мост Unity WebGL
2. [`types/ysdk.d.ts`](types/ysdk.d.ts) — сигнатуры
3. [`sdk/sdk-about.connect.md`](sdk/sdk-about.connect.md) — `/sdk.js`
4. Страницы `sdk/` по фиче
5. [`requirements/sdk-methods.md`](requirements/sdk-methods.md) — пункт 1.19
6. [`concepts/local-launch.md`](concepts/local-launch.md), [`console/debug-panel.md`](console/debug-panel.md)

## Содержимое папки

```
Docs/YandexGamesSDK/
  AGENT.md                  мост Unity WebGL → ysdk
  README.md                 этот файл
  llms.txt                  полный индекс документации платформы
  types/ysdk.d.ts           контракт API (@types/ysdk@1.2.0)
  sdk.md                    обзор SDK
  sdk/                      нативный JS API + восстановленный connect
  concepts/                 быстрый старт, требования, local-launch
  console/                  debug-панель
  requirements/             пояснения к частым пунктам модерации
```
