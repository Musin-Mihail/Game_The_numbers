---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-environment.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-environment.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-environment.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-environment.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-environment.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-environment.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-environment.md
  - href: ru/sdk/sdk-environment.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Переменные окружения

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

Вы можете получать информацию об окружении, в котором работает игра. Для этого используйте объект `environment`.

```javascript showLineNumbers
{
  [app](*key_app): {
    [id](*key_id): string;
  };
  [i18n](*key_i18n): {
    [lang](*key_lang2): string;
  };
  [payload?](*key_payload): string;
  [referrer?](*key_referrer): {
    [type](*key_type): "promo";
    [promoId](*key_promo_id): string;
    [intent?](*key_intent): string;
    [inappId?](*key_inapp_id): string;
  }
}
```

## Объект environment {#environment-object}

Содержит переменные окружения игры.

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `app` | `object` | Данные игры. ||
|| `i18n` | `object` | Интернационализация на сервисе. ||
|| `payload` | `string` | Значение параметра `payload` из адреса игры. Необязательный параметр. Например, для игры `https://yandex.ru/games/app/123?payload=test` значение `test` можно получить так: `ysdk.environment.payload`. ||
|#


### Структура app {#structure-app}

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `id` | `string` | Идентификатор игры. ||
|#


### Структура i18n {#structure-i18n}

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `lang` | `string` | [Язык интерфейса Яндекс Игр](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains.md) в формате ISO 639-1. Например, `"tr"` означает, что игра сейчас запущена в турецком интерфейсе Яндекс Игр. Этот параметр нужно использовать для автоматического определения языка пользователя в игре ([пункт 2.14](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#2-14)). ||
|#

#### Пример {#i18n-example}

```javascript showLineNumbers
const ysdk = (await YaGames.init());
const lang = ysdk.environment.i18n.lang; // 'en', 'ru', ...
```


### Структура referrer {#structure-referrer}

Используйте `ysdk.environment.referrer`, чтобы обрабатывать переходы игроков из акционных баннеров в каталоге. Преимущества:

- **Для игрока**: после перехода по акции игрок сразу попадает на нужный экран — видит именно то предложение, ради которого кликнул. Это повышает конверсию и снижает отток на старте.
- **Для вас**: вы можете отслеживать эффективность каждой акции — сколько игроков перешло, сколько совершило покупку. Для сбора статистики дополнительно подключите [Яндекс Метрику](https://yandex.ru/dev/games/doc/ru/concepts/yandex-metrica.md).

#### Настройка {#referrer-setup}

1. В Консоли разработчика добавьте [промоакцию](https://yandex.ru/dev/games/doc/ru/console/promo-and-discounts.md). Платформа автоматически сгенерирует диплинк (deeplink) для акционных баннеров в каталоге.

    Пример ссылки с параметрами акции:

    ```text
    https://yandex.ru/games/app/{id}?lang=ru&referrer=promo&promo_id={PROMO_ID}&promo_intent={INTENT}&inapp_id={INAPP_ID}
    ```

    #|
    || **Параметр** | **Описание** | **Обязательность** | **Источник** ||
    || `referrer` | Говорит платформе, что переход из акции. |::{align="center"}
    ![SVG](../_images/icons/yes-button.svg) | Всегда `promo` ||
    || `promo_id` | ID акции для маршрутизации и аналитики. |::{align="center"}
    ![SVG](../_images/icons/yes-button.svg) | **ID** ||
    || `promo_intent` | Произвольная подсказка для маршрутизации внутри игры и аналитики. |  | **Intent** ||
    || `inapp_id` | ID инап-покупки, с которой связана акция в игре. |  | **ID инапа** ||
    |#

1. Добавьте в игру [обработку](#referrer-example) перехода: SDK передает параметры диплинка в объект `ysdk.environment.referrer` — используйте их, чтобы показать игроку нужный экран.

    #|
    || **Параметр** | **Тип** | **Описание** | **Источник** ||
    || `type` | `"promo"` | Показывает источник перехода. Отслеживайте `type: "promo"`, чтобы собирать аналитику по промоакциям. | `referrer=promo` ||
    || `promoId` | `string` | ID акции с вкладки **Промоакции** в Консоли. Используйте, чтобы настроить действие для конкретной акции и собрать по ней аналитику. | `promo_id` ||
    || `intent` | `string` | Произвольная строка, например `open_starter_pack`. Используйте, чтобы настроить типовое действие после перехода по акции и собрать по нему аналитику. Опциональный параметр. | `promo_intent` ||
    || `inappId` | `string` | [ID инап-покупки](https://yandex.ru/dev/games/doc/ru/console/purchases.md#add-purchases) с вкладки **Инапы** в Консоли. Используйте, чтобы открыть платформенный диалог покупки. Опциональный параметр (только для скидочных акций). | `inapp_id` ||
    |#

1. Проверьте переходы: на вкладке **Промоакции** в поле **Проверить переход** нажмите **В опубликованной версии** или **В черновике**. Если все настроено правильно, вы увидите экран с акцией: оффер, магазин или конкретную инап-покупку.

#### Пример {#referrer-example}

```javascript showLineNumbers
// 1. Инициализация SDK
const ysdk = await YaGames.init();

// 2. Получите referrer
const { referrer } = ysdk.environment;

// 3. Обработайте переход из акции
if (referrer?.type === 'promo') {
    if (referrer.inappId) {
        showPurchaseScreen(referrer.inappId);
    } else if (referrer.intent) {
        openScreen(referrer.intent);
    }
}
```

#### Типовые сценарии {#referrer-scenarios}

#|
|| **Сценарий** | **Целевая аудитория** | **Цель** | **Пример** ||
|| Скидка на товар | Неплатящие игроки | Первый платеж | `promo_id=SPRING_DISCOUNT`<br>`promo_intent=open_starter_pack`<br>`inapp_id=starter_pack_001`

→ `showPurchaseScreen()` ||
|| VIP-оффер или открытие магазина | Платящие игроки | Рост среднего чека | `promo_id=VIP_PROMO`<br>`promo_intent=open_shop`

→ `openShop()` ||
|| Сезонная акция | Все активные игроки | Удержание | `promo_id=SALE_SPRING_2026`

→ базовый флоу, баннер ||
|#


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

[*key_app]: Данные игры.

[*key_id]: Идентификатор игры.

[*key_i18n]: Интернационализация на сервисе.

[*key_lang2]: [Язык интерфейса Яндекс Игр](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains.md#languages) в формате ISO 639-1. Например, `"tr"` означает, что игра сейчас запущена в турецком интерфейсе Яндекс Игр. Рекомендуем использовать этот параметр для определения языка пользователя в игре.

[*key_payload]: Значение параметра `payload` из адреса игры.
Необязательный параметр.
Например, для игры `https://yandex.ru/games/app/123?payload=test` значение `test` можно получить так: `ysdk.environment.payload`.

[*key_referrer]: Данные о переходе из промоакции. Присутствует, если игра открыта по диплинку из акционного баннера. `undefined` в другом случае.

[*key_type]: Источник перехода. Всегда `"promo"`.

[*key_promo_id]: ID акции с вкладки **Промоакции** в Консоли.

[*key_intent]: Произвольная строка, например `open_starter_pack`. Опциональный параметр.

[*key_inapp_id]: [ID инап-покупки](https://yandex.ru/dev/games/doc/ru/console/purchases.md#add-purchases) с вкладки **Инапы** в Консоли. Опциональный параметр (только для скидочных акций).