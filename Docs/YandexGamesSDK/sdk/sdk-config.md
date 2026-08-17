---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-config.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-config.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-config.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-config.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-config.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-config.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-config.md
  - href: ru/sdk/sdk-config.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Удаленная конфигурация

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

Чтобы получить удаленную конфигурацию флагов (Remote Config), используйте метод `ysdk.getFlags()` из SDK Яндекс Игр. Рекомендуем запрашивать флаги один раз на старте игры.

**Сигнатура и интерфейсы метода** {#signature-and-interfaces}

```typescript showLineNumbers
interface IFlags {
    [key: string]: string;
}

interface IClientFeature {
    name: string;
    value: string;
}

interface IGetFlagsParams {
    defaultFlags?: IFlags;
    clientFeatures?: IClientFeature[];
}

function getFlags(getFlagsParams: IGetFlagsParams = {}): IFlags {}
```

Принимает параметры:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `name` | `string` | Название клиентского параметра. ||
|| `value` | `string` | Значение клиентского параметра. ||
|| `defaultFlags` | `IFlags` | [Локальная конфигурация](#local-config): плоский объект с парами «ключ — значение». Соответствует удаленной конфигурации, [заданной](https://yandex.ru/dev/games/doc/ru/config.md) в Консоли. ||
|| `clientFeatures` | `IClientFeature[]` | Массив с [клиентскими параметрами](#client-params). Содержит [данные игрока](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#profile-data). ||
|#

</div>

#### Пример {#example-getflags}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const flags = await ysdk.getFlags(); // Метод возвращает объект с флагами.

// В логике игры можно добавить условие:
if (flags.difficult === 'hard') {
    // Включаем высокую сложность.
}
```


## Локальная конфигурация {#local-config}

{% note tip %}

Всегда добавляйте локальную конфигурацию флагов в код игры на случай, если не удастся получить удаленную конфигурацию с сервера (например, из-за проблем с интернет-соединением).

{% endnote %}

Чтобы добавить локальную конфигурацию (плоский объект, значения — строки), нужно передать ее в дополнительный параметр метода `ysdk.getFlags()`, в поле `defaultFlags`. Полученный в итоге объект является объединением удаленной и локальной конфигураций. Приоритет удаленной конфигурации выше.

#### Пример {#example-local-config}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const flags = await ysdk.getFlags({ defaultFlags: { difficult: 'easy' } });

if (flags.difficult === 'easy') {

}
```


## Клиентские параметры {#client-params}

Если ваша игра хранит [данные игрока](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md) (пройденные уровни, опыт, инап-покупки и т. д.), то их можно использовать в удаленной конфигурации. Подробнее о том, как настроить флаг в зависимости от условий, см. в разделе [Шаг 1. Создайте конфигурацию флагов](https://yandex.ru/dev/games/doc/ru/config.md#create-flag-config).

Клиентские параметры нужно передавать в виде массива в поле `clientFeatures` метода `ysdk.getFlags()`.

#### Пример {#example-client-params}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const player = await ysdk.getPlayer();

const payingStatus = player.getPayingStatus();

// Запрашиваем флаги с клиентским параметром статуса платежной активности пользователя.
const flags = await ysdk.getFlags({
    clientFeatures: [
        { name: 'payingStatus', value: payingStatus }
    ]
});
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
