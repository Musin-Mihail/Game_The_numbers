---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-about.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-about.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-about.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-about.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-about.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-about.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-about.md
  - href: ru/sdk/sdk-about.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Подключение и использование

<!-- source: ru/_includes/script-common.md -->
<!-- source: ru/_includes/script/index-js.md -->

<!-- endsource: ru/_includes/script/index-js.md -->

<!-- source: ru/_includes/script/requirements-js.md -->

<!-- endsource: ru/_includes/script/requirements-js.md -->

<!-- source: ru/_includes/script/image-modal-js.md -->

<!-- endsource: ru/_includes/script/image-modal-js.md -->


<!-- source: ru/_includes/script/neuroexpert-widget.md -->



<!-- endsource: ru/_includes/script/neuroexpert-widget.md -->
<!-- endsource: ru/_includes/script-common.md -->

## Подключение {#connect}

{% note alert %}

Чтобы ваша игра успешно прошла модерацию, укажите актуальный путь для подключения SDK Яндекс&nbsp;Игр:

- Если вы загружаете архив игры на сервер Яндекса через [Консоль разработчика](https://games.yandex.ru/console){.external}, укажите [относительный путь](#yandex-server). Это рекомендуемый вариант.
- Если вы используете интеграцию через свой домен, укажите [абсолютный путь](#iframe).

{% endnote %}

Подключить SDK Яндекс&nbsp;Игр можно двумя равноправными способами.

**Восстановленные HTML-примеры** (в исходном markdown теги `<script>` вырезаны санитизацией): [sdk-about.connect.md](sdk-about.connect.md).

- Через тег `script` в `index.html` (хостинг Яндекса):

    ```html
    <script src="/sdk.js"></script>
    ```

    Используйте атрибуты:

    - `async` — для неблокирующей загрузки.
    - `onload` — для выполнения кода после загрузки скрипта.

    Пример кода для запуска `initSDK` после загрузки скрипта. `initSDK` подразумевает [инициализацию SDK](#use):

    ```html showLineNumbers
    <!-- Yandex Games SDK -->
    <script src="/sdk.js" async onload="initSDK()"></script>
    ```

- Динамическая загрузка — см. [sdk-about.connect.md](sdk-about.connect.md).

## Использование {#use}

После загрузки скрипта инициализируйте SDK, используя метод `init()` объекта `YaGames`.

{% note tip %}

В `YaGames.init()` и [ysdk.getPayments()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-purchases.md#install) можно передать опциональный параметр `signed: boolean`, который предназначен для [защиты от накруток](https://yandex.ru/dev/games/doc/ru/sdk/sdk-purchases.md#signature). Выбор значения зависит от того, где обрабатываются платежи:

- Если на стороне клиента — вызовите методы без параметра `signed: boolean` или передайте `signed: false`. Методы покупок будут возвращать данные в открытом виде.
- Если на стороне сервера — передайте `signed: true`. В таком случае в ответах методов [payments.getPurchases()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-purchases.md#getpurchases) и [payments.purchase()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-purchases.md#payments-purchase) все данные возвращаются только в зашифрованном виде в параметре `signature`.

{% endnote %}

{% list tabs %}

- Обработка на стороне клиента

    Инициализация с параметром по умолчанию (`signed: false`):

    ```javascript
    const ysdk = await YaGames.init();
    ```

- Обработка на сервере

    Инициализация с параметром `signed: true`:

    ```javascript
    const ysdk = await YaGames.init({ signed: true });
    ```

{% endlist %}

&nbsp; {.empty}

## Проверка {#check}

{% note warning %}

Скрипт `/sdk.js` должен быть подключен до выполнения [YaGames.init()](#use).

{% endnote %}

Проверьте правильность подключения SDK с помощью лоадера:

1. Запустите игру с [debug-панелью](https://yandex.ru/dev/games/doc/ru/console/debug-panel.md):

   <!-- source: ru/_includes/requirements/start-debug-panel.md -->
   {% list tabs %}

   - Через Консоль разработчика
       1. Откройте [Консоль Яндекс Игр](https://games.yandex.ru/console){.external}.
       1. Выберите нужную игру.
       1. В левом верхнем углу нажмите **Открыть с debug-панелью**.

   - Через адресную строку
       1. Откройте нужную игру.
       1. Добавьте параметр `debug-mode=16` в конец адресной строки браузера.

          Пример ссылки: `https://yandex.ru/games/app/XXXX?debug-mode=16`, где `XXXX` — уникальный идентификатор игры.

   {% endlist %}
   <!-- endsource: ru/_includes/requirements/start-debug-panel.md -->

2. В левом нижнем углу проверьте значение индикатора [лоадера](https://yandex.ru/dev/games/doc/ru/console/debug-panel.md#loader):

    - `W` — ожидает инициализации.
    - `IT` — загрузчик SDK инициализирован верно.
    - `IF` — используется старый лоадер. Загрузите SDK в соответствии с [документацией](#connect).

## Решение проблем {#faq}

### Uncaught ReferenceError: YaGames is not defined {#yagames-not-defined}

Обратите внимание на порядок подключения скрипта `sdk`: он должен быть подключен до выполнения `YaGames.init().`

### Uncaught ReferenceError: ysdk is not defined {#ysdk-not-defined}

Вы попытались использовать методы SDK (реклама, покупки и т. д.) до момента инициализации SDK. Момент инициализации можно отследить в debug-режиме по сообщению `Initialized` в консоли. Чтобы контролировать порядок вызовов, добавьте инициализацию SDK перед вызовом метода:

```javascript showLineNumbers
const ysdk = await YaGames.init();

ysdk.adv.showFullscreenAdv();
```

### Пример подключения SDK {#connection-example}

```html showLineNumbers
<!-- Yandex Games SDK -->
<script src="/sdk.js"></script>
```

---

<!-- source: ru/_includes/sdk-support.md -->
{% note info %}

Сотрудники службы поддержки помогают разместить готовую игру на платформе Яндекс Игр. На прикладные вопросы о разработке и тестировании предметно ответят другие разработчики в [Сообществе в Телеграме](https://t.me/yagamedev){.telegram}.

{% endnote %}

Если при использовании SDK Яндекс Игр вы столкнулись с проблемой или у вас появился вопрос, обратитесь в службу поддержки:

<!-- source: ru/_includes/button-chat.md -->
<a href="https://yandex.ru/chat/#/user/774df508-c12d-9d6e-6a27-5e3fc522016a">
  <span class="button">Написать в чат</span>
</a>
<!-- endsource: ru/_includes/button-chat.md -->
<!-- endsource: ru/_includes/sdk-support.md -->
