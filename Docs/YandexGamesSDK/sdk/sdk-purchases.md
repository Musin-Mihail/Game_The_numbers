---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-purchases.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-purchases.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-purchases.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-purchases.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-purchases.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-purchases.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-purchases.md
  - href: ru/sdk/sdk-purchases.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Инап-покупки

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

Вы можете получать доход, предоставив пользователям возможность совершать покупки в игре. Например, дополнительное время на прохождение уровня или аксессуары для игрового персонажа. Для этого:

- [Подключите инап-покупки](https://yandex.ru/dev/games/doc/ru/console/purchases.md#connect) в Консоли разработчика.
- Настройте в SDK возможность работы с покупками.
- Добавьте [проверку необработанных покупок](#check-purchases).
- [Протестируйте покупки](https://yandex.ru/dev/games/doc/ru/console/purchases.md#test).

{% note alert %}

Тестировать покупки можно только после подключения их [консумирования](#check-purchases). Иначе могут появиться необработанные платежи, которые сделают прохождение модерации [невозможным](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-13-1).

{% endnote %}

## Условия подключения {#conditions}

Перед работой с SDK проверьте схему сотрудничества. Для этого в [Консоли разработчика](https://games.yandex.ru/console){.external} перейдите в раздел **Профиль** и проверьте значение поля **Единая лицензионная схема**:

{% list tabs %}

- Не подключена

  Подключите монетизацию и покупки:

  1. Подключите [рекламную монетизацию](https://yandex.ru/dev/games/doc/ru/console/adv-monetization.md#enable-int-monetization). В [партнерском интерфейсе РСЯ](https://partner.yandex.ru/){.external} укажите реквизиты для выплат по рекламе и покупкам. После проверки данных статус договора в интерфейсе РСЯ в разделе **Дополнительно → Документы** изменится на **Оферта акцептована**.
  2. Отправьте письмо с запросом на подключение на почту [games-partners@yandex-team.ru](mailto:games-partners@yandex-team.ru){.external}. В письме укажите:

      - название игры;
      - идентификатор (ID) игры.

      {% note tip %}

      Отправьте запрос как можно раньше, вы можете сделать это до загрузки архива игры или добавления покупок.

      {% endnote %}

      Вам придет ответное письмо от [games-partners@yandex-team.ru](mailto:games-partners@yandex-team.ru){.external} с подтверждением, что покупки разрешены.

  Дальнейшие шаги инструкции см. в разделе [Подключить покупки](https://yandex.ru/dev/games/doc/ru/console/purchases.md#connect).

- Подключена

  Покупки во всех ваших играх включены автоматически. Перейдите к [инициализации](#install).

{% endlist %}

## Инициализация {#install}

Чтобы игроки могли совершать инап-покупки, используйте объект `payments`. Вы можете:

- Обратиться напрямую к `ysdk.payments`. Покупки инициализируются при первом вызове любого из методов объекта, из-за чего первый вызов может быть чуть медленнее.

- Инициализировать объект с помощью метода `ysdk.getPayments()`. Он предзагружает данные, необходимые для методов `payments`. Так их первый вызов не будет замедлен.

{% note alert %}

В `YaGames.init()` и `ysdk.getPayments()` можно передать опциональный параметр `signed: boolean`, который предназначен для [защиты от накруток](#signature). Выбор значения зависит от того, где обрабатываются платежи:

- Если на стороне клиента — вызовите методы без параметра или передайте `signed: false`. Методы покупок будут возвращать данные в открытом виде.

- Если на стороне сервера — передайте `signed: true`. В таком случае в ответах методов [payments.getPurchases()](#getpurchases) и [payments.purchase()](#payments-purchase) все данные возвращаются только в зашифрованном виде в параметре `signature`.

{% endnote %}

{% list tabs group=purchases %}

- Обработка на клиенте

  Инициализация с параметром по умолчанию (`signed: false`).

  **Способ 1: Упрощенный**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  const payments = ysdk.payments;
  ```

  **Способ 2: Предзагрузка через getPayments()**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  try {
      const payments = await ysdk.getPayments();
  } catch (err) {
      // [Покупки недоступны](*key_explanation).
  }
  ```

- Обработка на сервере

  Инициализация с параметром `signed: true`.

  **Способ 1: При инициализации SDK**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init({ signed: true });

  const payments = ysdk.payments;
  ```

  **Способ 2: Более тонкая настройка через getPayments()**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  try {
      const payments = await ysdk.getPayments({ signed: true });
  } catch (err) {
      // [Покупки недоступны](*key_explanation).
  }
  ```

{% endlist %}

&nbsp; {.empty}

## Активация процесса покупки {#payments-purchase}

Чтобы активировать инап-покупку, используйте метод `payments.purchase()`. Он открывает фрейм с платежным шлюзом.

**Сигнатура метода**

```typescript showLineNumbers
function purchase(data: {
    id: string;
    developerPayload?: string;
}) => Promise<IPurchase | ISign> {}
```

Принимает параметры:

<div class="full-width-table table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `id` | `string` | Идентификатор товара, который [задан в Консоли разработчика](https://yandex.ru/dev/games/doc/ru/console/purchases.md#add-purchases). ||
|| `developerPayload` | `string` | Опциональный параметр. Содержит дополнительную информацию о покупке, которую вы хотите передавать на свой сервер (будет передана в параметре [signature](*key_sign)). ||
|#

</div>

{% list tabs group=purchases %}

- Обработка на клиенте

  [Инициализация](#install) с параметром по умолчанию (`signed: false`).

  Возвращает `Promise<IPurchase>` с информацией о покупке.

  ```typescript showLineNumbers
  interface IPurchase {
      productID: string;
      purchaseToken: string;
      developerPayload: string;
  }
  ```

  Содержит:

  <div class="table-25 table-2c25">

  #|
  || **Параметр** | **Тип** | **Описание** ||
  || `productID` | `string` | Идентификатор товара. ||
  || `purchaseToken` | `string` | Токен для [использования покупки](#consumepurchase). ||
  || `developerPayload` | `string` | Дополнительная информация о покупке. ||
  |#

  </div>

- Обработка на сервере

  [Инициализация](#install) с параметром `signed: true`.

  Возвращает `Promise<ISign>`.

  ```typescript showLineNumbers
  interface ISign {
      signature: string;
  }
  ```

  Содержит:

  <div class="full-width-table table-25 table-2c25">

  #|
  || **Параметр** | **Тип** | **Описание** ||
  || `signature` | `string` | Зашифрованные данные о покупке и подпись для [проверки подлинности игрока](#purchase-data-example). ||
  |#

  </div>

{% endlist %}

После успешного совершения покупки `Promise` разрешается со статусом `fulfilled`. Если игрок не совершил покупку и закрыл окно, `Promise` отклоняется со статусом `rejected`.

{% note alert %}

Нестабильная работа интернета может привести к ситуации, когда игрок совершил покупку, но она не была обработана в игре. Чтобы избежать этого, для обработки покупок применяйте методы, описанные в разделах [Проверка необработанных покупок](#check-purchases) и [payments.consumePurchase()](#consumepurchase).

Отказ от следования этим инструкциям может привести к отключению инап-покупок в приложении или снятию его с публикации.

{% endnote %}

Пользователь может совершить покупку без авторизации, но мы рекомендуем предлагать ему [войти в аккаунт](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#open-auth-dialog) заранее или при совершении покупки.

#### Пример {#purchase-example}

В общем случае:

```javascript showLineNumbers
const ysdk = await YaGames.init();

try {
    const purchase = await ysdk.payments.purchase({ id: 'gold500' });
} catch (err) {
    // Покупка не удалась: в Консоли разработчика не добавлен товар с таким id,
    // пользователь не авторизовался, передумал и закрыл окно оплаты,
    // истекло отведенное на покупку время, не хватило денег и т. д.
}
```

С использованием опционального параметра `developerPayload`:

```javascript showLineNumbers
const ysdk = await YaGames.init();

try {
    const purchase = await ysdk.payments.purchase({ id: 'gold500', developerPayload: '{serverId:42}' });
} catch (err) {
    // Обработка ошибки покупки.
}
```

## Получение списка купленных товаров {#getpurchases}

Используйте метод `payments.getPurchases()`, чтобы:

- узнать, какие покупки игрок уже совершил;

- проверить наличие [необработанных покупок](#check-purchases);

- обработать постоянные покупки.

**Сигнатура метода**

```typescript
function getPurchases(): Promise<IPurchase[] | ISign> {}
```

{% list tabs group=purchases %}

- Обработка на клиенте

  [Инициализация](#install) с параметром по умолчанию (`signed: false`).

  Возвращает `Promise<IPurchase[]>` с массивом покупок. Каждый элемент массива имеет тот же формат, что и покупка, возвращаемая методом [payments.purchase()](#payments-purchase).

  **Пример**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  let SHOW_ADS = true;

  try {
      const purchases = await ysdk.payments.getPurchases();

      if (purchases.some(purchase => purchase.productID === 'disable_ads')) {
          SHOW_ADS = false;
      }
  } catch (err) {
      // Ошибка получения списка покупок. Выбрасывает исключение PAYMENT_FAILURE.
  }
  ```

- Обработка на сервере

  [Инициализация](#install) с параметром `signed: true`.

  Возвращает `Promise<ISign>`.

  Содержит:

  <div class="full-width-table table-25 table-2c25">

  #|
  || **Параметр** | **Тип** | **Описание** ||
  || `signature` | `string` | Зашифрованные данные о покупке и подпись для [проверки подлинности игрока](#purchase-data-example). ||
  |#

  </div>

  **Пример**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init({ signed: true });

  try {
      const purchases = await ysdk.payments.getPurchases();
      // Отправляем список покупок на сервер.
      const response = await fetch('https://your.game.server/handlePurchases', {
          method: 'POST',
          headers: { 'Content-Type': 'text/plain' },
          body: purchases.signature
      });
  } catch (err) {
      // Ошибка получения списка покупок или их обработки.
  }
  ```

{% endlist %}

&nbsp; {.empty}

## Получение каталога всех товаров {#getcatalog}

Чтобы получить список доступных покупок и их стоимость, используйте метод `payments.getCatalog()`.

**Сигнатура метода**

```typescript showLineNumbers
interface IProduct {
    id: string;
    title: string;
    description: string;
    imageURI: string;
    price: string;
    priceValue: string;
    priceCurrencyCode: string;
    getPriceCurrencyImage(size: 'small' | 'medium' | 'svg'): string;
}

function getCatalog(): Promise<IProduct[]> {}
```

Метод возвращает список доступных для пользователя товаров. Формируется из таблицы на вкладке **Инап-покупки** [Консоли разработчика](https://games.yandex.ru/console){.external}. Каждый `IProduct` содержит свойства: {#product-characteristics}

<div class="table-25 table-2c25">

#|
|| **Свойство** | **Тип** | **Описание** ||
|| `id` | `string` | Идентификатор товара. ||
|| `title` | `string` | Название товара. ||
|| `description` | `string` | Описание товара. ||
|| `imageURI` | `string` | URL изображения товара. ||
|| `price` | `string` | Стоимость товара в формате `<цена> <код валюты>`. ||
|| `priceValue` | `string` | Стоимость товара в формате `<цена>`. ||
|| `priceCurrencyCode` | `string` | Код валюты. ||
|| `getPriceCurrencyImage(size)` | `string` | Метод получения адреса иконки валюты в зависимости от параметра размера иконки. Возможные значения:

- `small` (по умолчанию) — получение маленькой иконки.

- `medium` — получение иконки среднего размера.

- `svg` — получение иконки в векторе. ||
|#

</div>

{% note warning %}

Портальная валюта должна определяться автоматически ([пункт 1.13.2](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-13-2)). Для этого берите ее название и иконку из свойств `IProduct`. Подробнее см. в разделе [Автоматическое определение портальной валюты](https://yandex.ru/dev/games/doc/ru/requirements/1/13.md#currency-detection).

{% endnote %}

#### Пример {#getcatalog-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

let gameShop = [];

try {
    const purchases = await ysdk.payments.getPurchases();

    gameShop = purchases;
} catch (err) {
    // Ошибка получения списка покупок.
}
```

## Обработка покупки и начисление внутриигровой валюты {#processing-crediting}

Существуют два типа покупок:

- Постоянные (например, отключение рекламы). Для их обработки применяйте метод [payments.getPurchases()](#getpurchases).
- Используемые (например, внутриигровая валюта). Для их обработки применяйте метод `payments.consumePurchase()`.

#### payments.consumePurchase() {#consumepurchase}

{% note alert %}

После вызова метода `payments.consumePurchase()` обработанная покупка удаляется без возможности восстановления. Поэтому сначала модифицируйте данные игрока методами [player.setData()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#ingame-data), [player.setStats()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#ingame-data) или [player.incrementStats()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#ingame-data), а затем обрабатывайте покупку.

{% endnote %}

**Сигнатура метода**

```typescript
function consumePurchase(purchaseToken: string): Promise<void> {}
```

Принимает `purchaseToken`, возвращаемый методами [payments.purchase()](#payments-purchase) и [payments.getPurchases()](#getpurchases). Если обработка прошла успешно, `Promise` разрешается со статусом `fulfilled`, если возникла ошибка — отклоняется со статусом `rejected`.

#### Пример {#consumepurchase-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

function addGold(value) {
    return ysdk.player.incrementStats({ gold: value });
}

try {
    const purchase = await ysdk.payments.purchase({ id: 'gold500' });

    await addGold(500);

    await ysdk.payments.consumePurchase(purchase.purchaseToken);
} catch (err) {
    // Обработка ошибки обработки используемой покупки.
}
```

## Проверка необработанных покупок {#check-purchases}

{% note alert %}

Эта проверка обязательна для прохождения модерации ([пункт 1.13.1](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-13-1)), поэтому важно настроить ее даже для тестовых покупок. Если добавить в игру покупки и протестировать их до настройки консумирования, то после тестов могут остаться необработанные платежи, которые сделают прохождение модерации невозможным.

{% endnote %}

Если во время совершения инап-покупки у пользователя отключился интернет или ваш сервер был недоступен, покупка может остаться необработанной. Чтобы этого избежать, проверяйте наличие необработанных покупок с помощью метода [payments.getPurchases()](#getpurchases), например, при каждом запуске игры.

{% list tabs group=purchases %}

- Обработка на клиенте

  [Инициализация](#install) с параметром по умолчанию (`signed: false`).

  **Пример**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  async function handlePurchase(purchase) {
      if (purchase.productID === 'gold500') {
          await ysdk.player.incrementStats({ gold: 500 });

          await ysdk.payments.consumePurchase(purchase.purchaseToken);
      }
  }

  const purchases = await ysdk.payments.getPurchases().then(purchases => purchases.forEach(consumePurchase));

  for (let purchase of purchases) {
      await handlePurchase(purchase);
  }
  ```

- Обработка на сервере

  [Инициализация](#install) с параметром `signed: true`.

  **Пример**

  ```javascript showLineNumbers
  const ysdk = await YaGames.init({ signed: true });

  try {
      const purchases = await ysdk.payments.getPurchases();
      // Отправляем список покупок на сервер.
      const response = await fetch('https://your.game.server/handlePurchases', {
          method: 'POST',
          headers: { 'Content-Type': 'text/plain' },
          body: purchases.signature
      });
  } catch (err) {
      // Ошибка получения списка покупок или их обработки.
  }
  ```

{% endlist %}

&nbsp; {.empty}

## Защита от накруток {#signature}

Чтобы обезопасить себя от возможных накруток показателей в игре, обрабатывайте покупки на стороне сервера:

1. Инициализируйте `YaGames.init()` или `ysdk.getPayments()` с параметром `{ signed: true }`.
2. Полученную подпись в ответах [payments.purchase()](#payments-purchase) и [payments.getPurchases()](#getpurchases) передайте на свой сервер, расшифруйте ее с помощью [секретного ключа](#key-example).
3. На своем сервере начислите игроку полученные в игре предметы.

```javascript showLineNumbers
function serverPurchase(signature) {
    return fetch('https://your.game.server/handlePurchase', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: signature
    });
}

// Убедитесь что покупки инициализированы с параметром { signed: true }.
const ysdk = await YaGames.init({ signed: true });

try {
    const purchase = await ysdk.payments.purchase({ id: 'gold500' });

    // Начисляем на сервере 500 золотых...
    await serverPurchase(purchase.signature);
} catch (err) {
    // Ошибка покупки.
}
```

Параметр `signature` передаваемого на сервер запроса содержит данные о покупке и подпись. Представляет собой две строки в кодировке `base64`: `<подпись>.<JSON с данными о покупке>`.

#### Пример signature {#signature-example}

```text
hQ8adIRJWD29Nep+0P36Z6edI5uzj6F3tddz6Dqgclk=.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZEF0IjoxNTcxMjMzMzcxLCJyZXF1ZXN0UGF5bG9hZCI6InF3ZSIsImRhdGEiOnsidG9rZW4iOiJkODVhZTBiMS05MTY2LTRmYmItYmIzOC02ZDJhNGNhNDQxNmQiLCJzdGF0dXMiOiJ3YWl0aW5nIiwiZXJyb3JDb2RlIjoiIiwiZXJyb3JEZXNjcmlwdGlvbiI6IiIsInVybCI6Imh0dHBzOi8veWFuZGV4LnJ1L2dhbWVzL3Nkay9wYXltZW50cy90cnVzdC1mYWtlLmh0bWwiLCJwcm9kdWN0Ijp7ImlkIjoibm9hZHMiLCJ0aXRsZSI6ItCR0LXQtyDRgNC10LrQu9Cw0LzRiyIsImRlc2NyaXB0aW9uIjoi0J7RgtC60LvRjtGH0LjRgtGMINGA0LXQutC70LDQvNGDINCyINC40LPRgNC1IiwicHJpY2UiOnsiY29kZSI6IlJVUiIsInZhbHVlIjoiNDkifSwiaW1hZ2VQcmVmaXgiOiJodHRwczovL2F2YXRhcnMubWRzLnlhbmRleC5uZXQvZ2V0LWdhbWVzLzE4OTI5OTUvMmEwMDAwMDE2ZDFjMTcxN2JkN2EwMTQ5Y2NhZGM4NjA3OGExLyJ9fX0=
```

#### Пример передаваемых данных о покупке (в формате `JSON`) {#purchase-data-example}

{% note warning %}

Формат данных параметра `signature` в функции `serverPurchase(signature)` отличается от используемого в методе [payments.getPurchases()](#getpurchases).

В методе `payments.getPurchases()` параметр `signature` содержит массив объектов покупок в поле `data`. В функции `serverPurchase(signature)` — объект покупки.

{% endnote %}

```json showLineNumbers
{
  "algorithm": "HMAC-SHA256",
  "issuedAt": 1571233371,
  "requestPayload": "qwe",
  "data": {
    "token": "d85ae0b1-9166-4fbb-bb38-6d2a4ca4416d",
    "status": "waiting",
    "errorCode": "",
    "errorDescription": "",
    "url": "https://yandex.ru/games/sdk/payments/trust-fake.html",
    "product": {
      "id": "noads",
      "title": "Без рекламы",
      "description": "Отключить рекламу в игре",
      "price": {
        "code": "YAN",
        "value": "49"
      },
      "imagePrefix": "https://avatars.mds.yandex.net/get-games/1892995/2a0000016d1c1717bd7a0149ccadc86078a1/"
    },
    "developerPayload": "TEST DEVELOPER PAYLOAD"
  }
}
```

#### Пример секретного ключа {#key-example}

`t0p$ecret`

Секретный ключ для проверки подписи является уникальным для игры. Формируется автоматически при создании покупок в [Консоли разработчика](https://games.yandex.ru/console){.external}. Ключ отображается на вкладке **Инап-покупки** → **Настройки**.

#### Пример проверки подписи на сервере {#server-check-example}

{% list tabs %}

* Python 3

    ```python showLineNumbers
    import hashlib
    import hmac
    import base64
    import json

    usedTokens = {}

    key = 't0p$ecret' # Держите ключ в секрете.
    secret = bytes(key, 'utf-8')
    signature = 'hQ8adIRJWD29Nep+0P36Z6edI5uzj6F3tddz6Dqgclk=.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZEF0IjoxNTcxMjMzMzcxLCJyZXF1ZXN0UGF5bG9hZCI6InF3ZSIsImRhdGEiOnsidG9rZW4iOiJkODVhZTBiMS05MTY2LTRmYmItYmIzOC02ZDJhNGNhNDQxNmQiLCJzdGF0dXMiOiJ3YWl0aW5nIiwiZXJyb3JDb2RlIjoiIiwiZXJyb3JEZXNjcmlwdGlvbiI6IiIsInVybCI6Imh0dHBzOi8veWFuZGV4LnJ1L2dhbWVzL3Nkay9wYXltZW50cy90cnVzdC1mYWtlLmh0bWwiLCJwcm9kdWN0Ijp7ImlkIjoibm9hZHMiLCJ0aXRsZSI6ItCR0LXQtyDRgNC10LrQu9Cw0LzRiyIsImRlc2NyaXB0aW9uIjoi0J7RgtC60LvRjtGH0LjRgtGMINGA0LXQutC70LDQvNGDINCyINC40LPRgNC1IiwicHJpY2UiOnsiY29kZSI6IlJVUiIsInZhbHVlIjoiNDkifSwiaW1hZ2VQcmVmaXgiOiJodHRwczovL2F2YXRhcnMubWRzLnlhbmRleC5uZXQvZ2V0LWdhbWVzLzE4OTI5OTUvMmEwMDAwMDE2ZDFjMTcxN2JkN2EwMTQ5Y2NhZGM4NjA3OGExLyJ9fX0='

    sign, data = signature.split('.')
    message = base64.b64decode(data)

    purchaseData = json.loads(message)
    result = base64.b64encode(hmac.new(secret, message, digestmod=hashlib.sha256).digest())
    if result.decode('utf-8') == sign:
      print('Signature check ok!')

      if not purchaseData['data']['token'] in usedTokens:
        usedTokens[purchaseData['data']['token']] = True # Используйте базу данных.
        print('Double spend check ok!')

        print('Apply purchase:', purchaseData['data']['product'])
        # Здесь можно безопасно начислить купленное.
    ```

* Node.js

    ```javascript showLineNumbers
    const crypto = require('crypto');

    const usedTokens = {};

    const key = 't0p$ecret'; // Держите ключ в секрете.
    const signature = 'hQ8adIRJWD29Nep+0P36Z6edI5uzj6F3tddz6Dqgclk=.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZEF0IjoxNTcxMjMzMzcxLCJyZXF1ZXN0UGF5bG9hZCI6InF3ZSIsImRhdGEiOnsidG9rZW4iOiJkODVhZTBiMS05MTY2LTRmYmItYmIzOC02ZDJhNGNhNDQxNmQiLCJzdGF0dXMiOiJ3YWl0aW5nIiwiZXJyb3JDb2RlIjoiIiwiZXJyb3JEZXNjcmlwdGlvbiI6IiIsInVybCI6Imh0dHBzOi8veWFuZGV4LnJ1L2dhbWVzL3Nkay9wYXltZW50cy90cnVzdC1mYWtlLmh0bWwiLCJwcm9kdWN0Ijp7ImlkIjoibm9hZHMiLCJ0aXRsZSI6ItCR0LXQtyDRgNC10LrQu9Cw0LzRiyIsImRlc2NyaXB0aW9uIjoi0J7RgtC60LvRjtGH0LjRgtGMINGA0LXQutC70LDQvNGDINCyINC40LPRgNC1IiwicHJpY2UiOnsiY29kZSI6IlJVUiIsInZhbHVlIjoiNDkifSwiaW1hZ2VQcmVmaXgiOiJodHRwczovL2F2YXRhcnMubWRzLnlhbmRleC5uZXQvZ2V0LWdhbWVzLzE4OTI5OTUvMmEwMDAwMDE2ZDFjMTcxN2JkN2EwMTQ5Y2NhZGM4NjA3OGExLyJ9fX0=';

    const [sign, data] = signature.split('.');
    const purchaseDataString = Buffer.from(data, 'base64').toString('utf8');
    const hmac = crypto.createHmac('sha256', key);

    hmac.update(purchaseDataString);

    const purchaseData = JSON.parse(purchaseDataString);

    if (sign === hmac.digest('base64')) {
      console.log('Signature check ok!');

      if (!usedTokens[purchaseData.data.token]) {
        usedTokens[purchaseData.data.token] = true; // Используйте базу данных.
        console.log('Double spend check ok!');

        console.log('Apply purchase:', purchaseData.data.product);
        // Здесь можно безопасно начислить купленное.
      }
    }
    ```

{% endlist %}

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

[*key_id]: `id: string` — идентификатор товара, который [задан в Консоли разработчика](https://yandex.ru/dev/games/doc/ru/console/purchases.md#connect).

[*key_developerPayload]: `developerPayload: string` — опциональный параметр. Дополнительная информация о покупке, которую вы хотите передавать на свой сервер (будет передана в параметре `signature`).

[*key_sign]: Параметр `signature` передаваемого на сервер запроса содержит данные о покупке и подпись. Представляет собой две строки в кодировке `base64`: `<подпись>.<JSON с данными о покупке>`.

[*key_purchaseToken]: `purchaseToken: string` — токен, возвращаемый методами [payments.purchase()](#payments-purchase) и [payments.getPurchases()](#getpurchases).

[*key_payments]: Обращайтесь напрямую к `ysdk.payments`, если вы не инициализировали покупки с помощью `ysdk.getPayments()`.

[*key_explanation]: - [Включите монетизацию](https://yandex.ru/dev/games/doc/ru/console/purchases.md#connect).
- В [Консоли разработчика](https://games.yandex.ru/console){.external} перейдите на вкладку **Инап-покупки** и убедитесь, что:
    - присутствует таблица хотя бы с одним внутриигровым товаром;
    - отображается надпись **Покупки подключены**.