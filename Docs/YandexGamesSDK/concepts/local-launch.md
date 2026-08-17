# Запуск с локального сервера

Снимок: https://yandex.ru/dev/games/doc/ru/concepts/local-launch.md (2026-08-18).

После запуска с localhost доступны все функции SDK. Два режима: prod (реальная платформа) и dev (моки).

## Prod-окружение

Нужны аккаунт и черновик игры. Игра открывается на yandex.ru/games, настоящая реклама, Паспорт, облако, покупки с сервера. Debug: `&debug-mode=16`.

Самостоятельно: локальный HTTPS `localhost` → черновик → `?game_url=https://localhost` (только домен `localhost`).

Через npm:

```console
npm install -g @yandex-games/sdk-dev-proxy
npx @yandex-games/sdk-dev-proxy -p <папка с билдом> --app-id=<ID игры>
```

Или прокси к уже запущенному серверу: `-h <адрес>`. Параметры: `--port` (8080), `--csp`, `--tld`, `--dev-mode`, `--log`.

## Dev-окружение

Регистрация не нужна. `--dev-mode=true`. Реклама и авторизация — заглушки (callbacks как в prod). Сохранения и покупки в `localStorage`. Каталог — файл `purchases-catalog.json` в корне билда.

```console
npx @yandex-games/sdk-dev-proxy -p <папка с билдом> --dev-mode=true
```

Моки в URL:

```text
localhost:8080?mocks={"canShowPrompt":true,"isAuthorized":true,"lockedOrientation":"landscape"}
```

Пример каталога:

```json
[
    {
        "description": "Disable action counters",
        "id": "disable_counters",
        "imageURI": "",
        "price": "100 RUB",
        "priceCurrencyCode": "RUB",
        "priceValue": "100",
        "title": "Disable counters"
    }
]
```

`id` должен совпадать с `GameConstants.DisableCountersProductId` в игре.
