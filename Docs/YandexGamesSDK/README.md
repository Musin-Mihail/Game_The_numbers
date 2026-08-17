# Yandex Games SDK (снимок документации)

Снимок официальной документации [Яндекс Игр](https://yandex.ru/dev/games/doc/ru/sdk.md) от **2026-08-18**.
Язык: русский. Источник markdown: `https://yandex.ru/dev/games/doc/ru/...`.

Индекс всех страниц платформы: [llms.txt](llms.txt). Онлайн: [llms.txt](https://yandex.ru/dev/games/doc/ru/llms.txt).

**Агенту, который убирает PluginYG2:** сначала [AGENT.md](AGENT.md). Это карта прямой интеграции. Папки `sdk/` и `unity/` сами по себе для этой задачи недостаточно.

## Какие из трёх ссылок нужны этой игре

Проект — Unity **WebGL** для каталога Яндекс Игр, не нативное мобильное приложение.

| Ссылка                                                                    | Что это                                       | Нужно ли                                                                          |
| ------------------------------------------------------------------------- | --------------------------------------------- | --------------------------------------------------------------------------------- |
| [Yandex Games SDK](https://yandex.ru/dev/games/doc/ru/sdk.md)             | Платформенный SDK: `YaGames.init()`, `ysdk.*` | **Да.** Локально: [`sdk/`](sdk/), контракт: [`types/ysdk.d.ts`](types/ysdk.d.ts). |
| [Quick start](https://yandex.ru/dev/games/doc/ru/concepts/quick-start.md) | Регистрация, черновик, модерация              | **Да, как чеклист публикации.**                                                   |
| [Yandex Mobile Ads Unity](https://ads.yandex.com/helpcenter/en/dev/unity) | Mobile Ads SDK 8 для Android/iOS              | **Нет.** Другой продукт.                                                          |

## PluginYG2 и «официальный Unity-плагин»

Раздел [sdk/unity](https://yandex.ru/dev/games/doc/ru/sdk/unity/install.md) — это **Plugin Your Games 2.0** (тот же PluginYG2 в `Assets/PluginYourGames`). Страницы `unity/*.md` — видеокурс, не нативный API.

Нативный контракт — JavaScript SDK в [`sdk/`](sdk/). PluginYG2 только оборачивает его в `YG2.*`. Прямая интеграция: читать [AGENT.md](AGENT.md), `sdk/`, `types/ysdk.d.ts`. Не читать `unity/` как спецификацию.

В исходном markdown теги `<script>` вырезаны санитизацией. Восстановленные примеры: [`sdk/sdk-about.connect.md`](sdk/sdk-about.connect.md). Для хостинга Яндекса: `/sdk.js` **до** `YaGames.init()`.

## С чего читать агенту

Игра уже использует: облачные сохранения, язык, лидерборд, interstitial, rewarded, покупки.

1. [AGENT.md](AGENT.md) — мост Unity WebGL и таблица `YG2` → `ysdk`
2. [`types/ysdk.d.ts`](types/ysdk.d.ts) — сигнатуры
3. [`sdk/sdk-about.connect.md`](sdk/sdk-about.connect.md) — `/sdk.js`
4. [`sdk/sdk-game-events.md`](sdk/sdk-game-events.md) — `LoadingAPI.ready()`, `GameplayAPI.start/stop`
5. [`sdk/sdk-player.md`](sdk/sdk-player.md) — профиль и облачные сохранения
6. [`sdk/sdk-environment.md`](sdk/sdk-environment.md) — язык (`i18n.lang`)
7. [`sdk/sdk-adv.md`](sdk/sdk-adv.md) — fullscreen / rewarded
8. [`sdk/sdk-leaderboard.md`](sdk/sdk-leaderboard.md)
9. [`sdk/sdk-purchases.md`](sdk/sdk-purchases.md)
10. [`requirements/sdk-methods.md`](requirements/sdk-methods.md) — пункт 1.19
11. [`concepts/local-launch.md`](concepts/local-launch.md), [`console/debug-panel.md`](console/debug-panel.md)

## Содержимое папки

```
Docs/YandexGamesSDK/
  AGENT.md                  playbook прямой интеграции (без PluginYG2)
  README.md                 этот файл
  llms.txt                  полный индекс документации платформы
  types/ysdk.d.ts           контракт API (@types/ysdk@1.2.0)
  sdk.md                    обзор SDK и плагины движков
  sdk/                      нативный JS API + восстановленный connect
  unity/                    видеокурс PluginYG2 — не использовать как API
  concepts/                 быстрый старт, требования, local-launch
  console/                  debug-панель
  requirements/             пояснения к частым пунктам модерации
```
