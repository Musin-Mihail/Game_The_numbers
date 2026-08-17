---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-adv.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-adv.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-adv.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-adv.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-adv.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-adv.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-adv.md
  - href: ru/sdk/sdk-adv.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Реклама

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

Вы можете получать доход от размещения рекламных блоков в своих играх. Для этого:
1. Изучите [особенности и рекомендации](#recommendations) по размещению рекламы.
1. Настройте [вызов рекламы](#settings) с помощью SDK.
1. [Подключите монетизацию](https://yandex.ru/dev/games/doc/ru/console/adv-monetization.md) в Консоли разработчика.

## Особенности и рекомендации по размещению рекламы {#recommendations}

- Размещайте вызов рекламы таким образом, чтобы пользователь понимал, что это не часть игры, а рекламный блок.
- Рекламу рекомендуется показывать после пользовательского действия или по таймеру, если прохождение уровня занимает больше 5 минут. Подробнее см. на странице [Расположение рекламы](https://yandex.ru/dev/games/doc/ru/requirements/4/4.md).
- Частота вызова [видеорекламы с вознаграждением (rewarded video)](#rewarded-video) не ограничена.
- Частота вызова полноэкранного блока рекламы управляется платформой [Яндекс Игры](https://yandex.ru/games/){.external}.


{% note alert %}

Рекламная сеть Яндекса считает случайные клики пользователей по блокам рекламы признаком рекламного фрода и снижает доход от рекламы в игре.

Чтобы избежать этого, не вызывайте показ рекламы, когда пользователь активно взаимодействует с игрой и может нажать на блок рекламы ненамеренно.

Пример неправильного вызова:

```javascript
setInterval(() => ysdk.adv.showFullscreenAdv(), 180000)
```

{% endnote %}


## Полноэкранный блок рекламы {#full-screen-block}

Блок с рекламой, который полностью закрывает приложение и показывается между запросом какой-то информации пользователем (например, при переходе на следующий уровень игры) и ее получением.

### ysdk.adv.showFullscreenAdv() {#show-fullscreen-adv}

Вызвать полноэкранный блок с рекламой.

**Сигнатура метода**

```typescript showLineNumbers
function showFullscreenAdv(callbacks?: {
    onOpen?: () => void;
    onClose?: (wasShown: boolean) => void;
    onError?: (error: object) => void;
}) => void {}
```

В параметре `callbacks` можно передать опциональные callback-функции:

#|
|| **Callback-функция** | **Описание** ||
|| `onOpen` | Вызывается при успешном открытии рекламы. ||
|| `onClose` | Вызывается при закрытии рекламы, после ошибки, а также, если реклама не открылась по причине слишком частого вызова. Используется с аргументом `wasShown` (тип: `boolean`), по значению которого можно узнать, была ли показана реклама. ||
|| `onError` | Вызывается при возникновении ошибки. Объект ошибки передается в callback-функцию. ||
|#

#### Пример {#full-screen-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

ysdk.adv.showFullscreenAdv({
    [callbacks](*key_callbacks): {
        [onOpen](*key_onOpen): () => console.log('Реклама открыта.'),
        [onClose](*key_onClose): (wasShown) => console.log(wasShown ? 'Показана и закрыта.' : 'Не показана.'),
        [onError](*key_onError): (error) => console.log('Ошибка вызова.'),
    }
})
```


## Видеореклама с вознаграждением (rewarded video) {#rewarded-video}

Блок с видеорекламой, за просмотр которого пользователь получает награду, например внутриигровую валюту.

### ysdk.adv.showRewardedVideo() {#show-rewarded-video}

Вызвать видеорекламу с вознаграждением.

**Сигнатура метода**

```typescript showLineNumbers
function showRewardedVideo(callbacks?: {
    onOpen?: () => void;
    onRewarded?: () => void;
    onClose?: (wasShown: boolean) => void;
    onError?: (error: object) => void;
}) => void {}
```

В параметре `callbacks` можно передать опциональные callback-функции:

#|
|| **Callback-функция** | **Описание** ||
|| `onOpen` | Вызывается при отображении видеорекламы на экране. ||
|| `onRewarded` | Вызывается, когда засчитывается просмотр видеорекламы. Укажите в этой функции, какую награду пользователь получит после просмотра. ||
|| `onClose` | Вызывается при закрытии видеорекламы. ||
|| `onError` | Вызывается при возникновении ошибки. Объект ошибки передается в callback-функцию. ||
|#


#### Пример {#rewarded-video-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

ysdk.adv.showRewardedVideo({
    [callbacks](*key_callbacks): {
        [onOpen](*key_onOpen): () => console.log('Реклама открыта.'),
        [onRewarded](*key_onRewarded): () => console.log('Пользователь получил награду.'),
        [onClose](*key_onClose2): (wasShown) => console.log(wasShown ? 'Показана и закрыта.' : 'Не показана.'),
        [onError](*key_onError): (error) => console.log('Ошибка вызова.'),
    }
})
```

## Стики-баннер {#sticky-banner}

Блок с рекламой, который показывается во время игры.

### Включение показа стики-баннера {#banner-on}

1. Откройте [Консоль разработчика](https://games.yandex.ru/console){.external} и перейдите на вкладку **Реклама**.
1. В блоке **Sticky-баннеры** настройте отображение баннеров:
    - Для мобильных устройств:
        - **Sticky-баннер в портретной ориентации** — выберите расположение **Внизу** или **Вверху**.
        - **Sticky-баннер в альбомной ориентации** — выберите расположение **Внизу**, **Вверху** или **Справа**.
    - Для компьютеров — включите опцию **Sticky-баннер на десктопе**. Баннер будет показываться справа.

### Управление показом стики-баннера {#banner-adv}

По умолчанию стики-баннер появляется при запуске игры и отображается всю сессию. Чтобы управлять показом стики-баннера с помощью методов SDK, в [Консоли разработчика](https://games.yandex.ru/console){.external} на вкладке **Реклама** включите опцию **Использовать API для показа sticky-баннера**.

#### ysdk.adv.getBannerAdvStatus() {#get-banner-adv-status}

Получить текущий статус стики-баннера.

**Сигнатура метода**

```typescript showLineNumbers
function getBannerAdvStatus(): Promise<{
    stickyAdvIsShowing: boolean;
    reason?: 'ADV_IS_NOT_CONNECTED' | 'UNKNOWN';
}> {}
```

Возвращает статус отображения стики-баннера `stickyAdvIsShowing`. Если баннер не показывается, также возвращает опциональное поле `reason`, которое указывает на причину отсутствия банера:

#|
|| **Причина** | **Описание** ||
|| `ADV_IS_NOT_CONNECTED` | Не подключены баннеры. ||
|| `UNKNOWN` | Ошибка показа рекламы на стороне Яндекса. ||
|#

#### ysdk.adv.showBannerAdv() {#show-banner-adv}

Показать стики-баннер.

**Сигнатура метода**

```typescript showLineNumbers
function showBannerAdv(): Promise<{
    stickyAdvIsShowing: boolean;
    reason?: 'ADV_IS_NOT_CONNECTED' | 'UNKNOWN';
}>
```

Возвращаемые значения аналогичны значениям [ysdk.adv.getBannerAdvStatus()](#get-banner-adv-status).

#### ysdk.adv.hideBannerAdv() {#hide-banner-adv}

Скрыть стики-баннер.

**Сигнатура метода**

```typescript showLineNumbers
function hideBannerAdv(): Promise<{
    stickyAdvIsShowing: boolean;
}>
```

Возвращает статус отображения стики-баннера `stickyAdvIsShowing`.

#### Пример {#sticky-banner-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const { stickyAdvIsShowing , reason } = await ysdk.adv.getBannerAdvStatus();

if (stickyAdvIsShowing) {
    // Реклама показывается.
} else if (reason) {
    // Реклама не показывается.
    console.log(reason);
} else {
    ysdk.adv.showBannerAdv();
}
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

[*key_callbacks]: `callbacks` — опциональные callback-функции. Настраиваются индивидуально для каждого рекламного блока.

[*key_onOpen]: `onOpen` — вызывается при отображении видеорекламы на экране.

[*key_onRewarded]: `onRewarded` — вызывается, когда засчитывается просмотр видеорекламы. Укажите в данной функции, какую награду пользователь получит после просмотра.

[*key_onClose]: `onClose` — вызывается при закрытии рекламы, после ошибки, а также, если реклама не открылась по причине слишком частого вызова. Используется с аргументом `wasShown` (тип: `boolean`), по значению которого можно узнать была ли показана реклама.

[*key_onClose2]: `onClose` — вызывается при закрытии видеорекламы.

[*key_onError]: `onError` — вызывается при возникновении ошибки. Объект ошибки передается в callback-функцию.