---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-leaderboard.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-leaderboard.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-leaderboard.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-leaderboard.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-leaderboard.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-leaderboard.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-leaderboard.md
  - href: ru/sdk/sdk-leaderboard.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Лидерборды

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

На странице игры вы можете показывать персонализированные лидерборды (таблицы лидеров) с результатами лучших игроков и местом авторизованного пользователя в рейтинге.

Чтобы запросы к лидербордам работали, выполните условия:

- в коде игры [подключите и настройте SDK](https://yandex.ru/dev/games/doc/ru/sdk/sdk-about.md#use), чтобы его объект был доступен через переменную `ysdk`;
- в Консоли разработчика [создайте](https://yandex.ru/dev/games/doc/ru/concepts/leaderboards.md) лидерборд.

{% note alert %}

Если в [Консоли](https://games.yandex.ru/console){.external} нет лидерборда с соответствующим именем в поле **Техническое название лидерборда**, запросы будут выдавать ошибку 404.

{% endnote %}



## Инициализация {#init}

Для вызова методов лидерборда обращайтесь напрямую к `ysdk.leaderboards`.

{% note alert %}

Инициализация объекта `lb` с помощью метода `ysdk.getLeaderboards()` устарела.

{% cut "Старые методы" %}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const lb = await ysdk.getLeaderboards();

// Соответствие вызова методов лидерборда старым и новым способами:
// lb.getLeaderboardDescription() → ysdk.leaderboards.getDescription()
// lb.setLeaderboardScore() → ysdk.leaderboards.setScore()
// lb.getLeaderboardPlayerEntry() → ysdk.leaderboards.getPlayerEntry()
// lb.getLeaderboardEntries() → ysdk.leaderboards.getEntries()
```

{% endcut %}

{% endnote %}



## Описание лидерборда {#description}

Чтобы получить описание лидерборда по его имени, используйте метод `ysdk.leaderboards.getDescription()`.

**Сигнатура метода**

```typescript showLineNumbers
interface ILeaderboardDescription {
    [appID](*key_appID): string;
    [default](*key_default): boolean;
    description: {
        [invert_sort_order](*key_invert_sort_order): boolean;
        score_format: {
            options: {
                [decimal_offset](*key_decimal_offset): number;
            };
            [type](*key_type): 'numeric' | 'time';
        };
        [sort_order](*key_sort_order): string;
    };
    [name](*key_name): string;
    [title](*key_title): Record<Locale, string>;
}

function getDescription(
    [leaderboardName](*key_name): string
): Promise<ILeaderboardDescription> {}
```

Принимает единственным параметром техническое название лидерборда `leaderboardName`. Возвращает объект с описанием лидерборда, который включает поля:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `appID` | `string` | Идентификатор приложения. ||
|| `default` | `boolean` | Если `true`, то лидерборд является основным. ||
|| `invert_sort_order` | `boolean` | Направление сортировки:
- `false` — по убыванию (на первых местах будут пользователи с наибольшим счетом);
- `true` — по возрастанию (на первых местах будут пользователи с наименьшим счетом). ||
|| `sort_order` | `string` | Направление сортировки в строковом формате:
- `'DESC'` — по убыванию;
- `'ASC'` — по возрастанию. ||
|| `decimal_offset` | `number` | Размер десятичной части счета. Например, при `decimal_offset: 2` число 1234 будет отображаться как 12.34. ||
|| `type` | `'numeric'` \| `'time'` | Тип результата лидерборда. Доступные значения: `numeric` (число), `time` (миллисекунды). ||
|| `name` | `string` | Имя лидерборда, указанное в Консоли в поле **Техническое название лидерборда**. ||
|| `title` | `Record<Locale, string>` | Список локализованных названий. Возможные коды языков перечислены на странице [Языки и домены](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains.md). ||
|#

</div>


#### Пример {#description-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const lb = await ysdk.leaderboards.getDescription('leaderboard2021');

console.log(lb);
```



## Новый результат {#set-score}

{% note alert %}

Запрос доступен только для авторизованных пользователей. Как проверить статус авторизации и вызвать диалог входа, см. в разделе [Авторизация пользователя](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#auth).

Перед отправкой запроса проверьте доступность метода с помощью `ysdk.isAvailableMethod('leaderboards.setScore')`. Метод возвращает `Promise<Boolean>`.

Чтобы результаты сохранялись для всех пользователей вне зависимости от авторизации, рекомендуем прописать кастомный лидерборд самостоятельно в коде приложения. Выбор технологии не ограничен.

{% endnote %}

Чтобы установить игроку новый результат, используйте метод `ysdk.leaderboards.setScore()`.

**Сигнатура метода**

```typescript showLineNumbers
function setScore(
    [leaderboardName](*key_name): string,
    [score](*key_score): number,
    [extraData](*key_extraData)?: string
): Promise<void> {}
```

Принимает параметры:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `leaderboardName` | `string` | Имя лидерборда, указанное в Консоли в поле **Техническое название лидерборда**. ||
|| `score` | `number` | Значение результата. Не может быть отрицательным, максимальное значение ограничено только логикой JavaScript. Если [тип лидерборда](*key_type) — `time`, то значение необходимо передавать в миллисекундах. ||
|| `extraData` | `string` | Описание пользователя. Необязательный параметр. ||
|#

</div>

{% note info %}

Запрос можно отправлять не чаще, чем 1 раз за 1 секунду, иначе он будет отклонен с ошибкой.

{% endnote %}


#### Пример {#set-score-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

await ysdk.leaderboards.setScore('leaderboard2021', 120);

await ysdk.leaderboards.setScore('leaderboard2021', 120, 'My favourite player!');
```



## Получение рейтинга {#get-entry}

{% note alert %}

Запрос доступен только для авторизованных пользователей. Как проверить статус авторизации и вызвать диалог входа, см. в разделе [Авторизация пользователя](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#auth).

Перед отправкой запроса проверьте доступность метода с помощью `ysdk.isAvailableMethod('leaderboards.getPlayerEntry')`. Метод возвращает `Promise<Boolean>`.

Чтобы результаты сохранялись для всех пользователей вне зависимости от авторизации, рекомендуем прописать кастомный лидерборд самостоятельно в коде приложения. Выбор технологии не ограничен.

{% endnote %}

Чтобы получить рейтинг пользователя, используйте метод `ysdk.leaderboards.getPlayerEntry()`.

**Сигнатура метода**

```typescript showLineNumbers
interface ILeaderboardEntry {
    [extraData](*key_extraData): string;
    [rank](*key_userRank): number;
    [score](*key_score): number;
    player: {
        [publicName](*key_publicName): string;
        [uniqueID](*key_uniqueID): string;
        [getAvatarSrc](*key_getAvatarSrc): (size?: 'small' | 'medium' | 'large') => string;
        [getAvatarSrcSet](*key_getAvatarSrcSet): (size?: 'small' | 'medium' | 'large') => string;
    }
}

function getPlayerEntry(
    [leaderboardName](*key_name): string
): Promise<ILeaderboardEntry> {}
```

Принимает единственным параметром техническое название лидерборда `leaderboardName`. Возвращает объект с рейтингом пользователя, который включает поля:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `score` | `number` | Значение результата. ||
|| `rank` | `number` | Позиция пользователя в лидерборде. ||
|| `extraData` | `string` | Описание пользователя. ||
|| `publicName` | `string` | Имя пользователя. ||
|| `uniqueID` | `string` | Уникальный идентификатор пользователя. ||
|| `getAvatarSrc` | `(size?: TSize) => string` | Возвращает URL портрета пользователя в заданном размере. Возможные значения `size`: `small`, `medium`, `large`. ||
|| `getAvatarSrcSet` | `(size?: TSize) => string` | Возвращает srcset портрета пользователя, который подходит для дисплеев Retina. Возможные значения `size`: `small`, `medium`, `large`. ||
|#

</div>

{% note info %}

Запрос можно отправлять не чаще, чем 60 раз за 5 минут, иначе он будет отклонен с ошибкой.

{% endnote %}


#### Пример {#get-entry-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

try {
    const res = await ysdk.leaderboards.getPlayerEntry('leaderboard2021');

    console.log(res);
} catch (err) {
    if (err.code === 'LEADERBOARD_PLAYER_NOT_PRESENT') {
        // Срабатывает, если у игрока нет записи в лидерборде.
    }
}
```



## Записи лидерборда {#get-entries}

Чтобы вывести рейтинг пользователей, используйте метод `ysdk.leaderboards.getEntries()`.

**Сигнатура метода**

```typescript showLineNumbers
interface ILeaderboardEntries {
    [leaderboard](*key_leaderboard): ILeaderboardDescription;
    [ranges](*key_ranges): {
        [start](*key_start): number;
        [size](*key_size): number;
    }[];
    [userRank](*key_userRank): number;
    [entries](*key_entries): ILeaderboardEntry[];
}

function getEntries(
    [leaderboardName](*key_name): string,
    options: {
        [includeUser](*key_includeUser)?: boolean;
        [quantityAround](*key_quantityAround)?: number;
        [quantityTop](*key_quantityTop)?: number;
    }
): Promise<ILeaderboardEntries> {}
```

Принимает техническое название лидерборда `leaderboardName` и опциональные параметры `options`:

<div class="table-25 table-2c25">

#|
|| **Опция** | **Тип** | **Описание** ||
|| `includeUser` | `boolean` | Определяет, включать ли авторизованного пользователя в ответ:
- `true` — включать в ответ.
- `false` (по умолчанию) — не включать. ||
|| `quantityAround` | `number` | Количество записей ниже и выше пользователя по лидерборду, которое нужно вернуть. Минимальное значение — 1, максимальное — 10. По умолчанию возвращается 5. ||
|| `quantityTop` | `number` | Количество записей из топа лидерборда. Минимальное значение — 1, максимальное — 20. По умолчанию возвращается 5. ||
|#

</div>

Возвращает объект с рейтингом пользователей `Promise<ILeaderboardEntries>`, который включает поля:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `leaderboard` | `ILeaderboardDescription` | [Описание лидерборда](#description) ||
|| `ranges` | `object[]` | Интервалы мест в ответе. ||
|| `start` | `number` | Место в рейтинге. Счет ведется с нуля, поэтому 1-е место считается нулевым элементом. ||
|| `size` | `number` | Количество запрошенных записей. Если данных не хватает, то может не соответствовать ответу. ||
|| `userRank` | `number` | Место пользователя в рейтинге. Если отсутствует, либо запрос на топ без включения пользователя, то равен 0. ||
|| `entries` | `ILeaderboardEntry[]` | Массив записей рейтинга. Запись идентична возвращаемому значению из метода [Получение рейтинга](#get-entry). ||
|#

</div>

{% note info %}

Запрос можно отправлять не чаще, чем 20 раз за 5 минут, иначе он будет отклонен с ошибкой.

{% endnote %}


#### Пример {#get-entries-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

// Получение 10 топ-игроков и 3 записей возле пользователя.
const entries = await ysdk.leaderboards.getEntries('leaderboard2021', {
    quantityTop: 10,
    includeUser: true,
    quantityAround: 3
});

console.log(entries);
```



## Ограничения методов {#limits}

<div class="table-25">

#|
|| **Метод** | **Описание** | **Лимит** | **Авторизация пользователей** ||
|| `ysdk.leaderboards.setScore()` | [Установить новый результат игрока](#set-score) | 1 запрос за 1 секунду | Обязательна ||
|| `ysdk.leaderboards.getPlayerEntry()` | [Вывести рейтинг одного пользователя](#get-entry) | 60 запросов за 5 минут | Обязательна ||
|| `ysdk.leaderboards.getEntries()` | [Получить рейтинг нескольких пользователей](#get-entries) | 20 запросов за 5 минут | Необязательна ||
|#

</div>

Лимит на остальные запросы: 20 запросов за 5 минут.



## Решение проблем {#faq}

{% note tip %}

При использовании сочетания методов `ysdk.isAvailableMethod()` и `ysdk.leaderboards.setScore()` неавторизованные пользователи не попадают в лидерборд и не видят свой прогресс. Чтобы результаты сохранялись для всех игроков, рекомендуем создать кастомный лидерборд в коде приложения. Выбор технологии не ограничен.

{% endnote %}

### Object already exists {#object-already-exists}

Ошибка возникает при попытке создать новый лидерборд с именем старого. Введите имя, которое раньше не использовалось.

### Пользователь скрыт {#player-hidden}

Надпись «Пользователь скрыт» отображается, если игрок не разрешил использовать свои аватар и имя. Доступ к данным пользователя зависит от настроек в его [профиле](https://yandex.ru/games/user){.external}. Подробнее см. в разделе [Инициализация](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#getplayer).

### Ошибка 404 {#leaderboard-404}

Если при вызове методов SDK для лидерборда возникает ошибка 404, проверьте, что в [Консоли разработчика](https://games.yandex.ru/console){.external} создан лидерборд с соответствующим именем в поле **Техническое название лидерборда**.



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

[*key_name]: Имя лидерборда, указанное в Консоли в поле **Техническое название лидерборда**.

[*key_appID]: Идентификатор приложения.

[*key_default]: Если `true`, то лидерборд является основным.

[*key_invert_sort_order]: Направление сортировки:
- `false` — по убыванию (на первых местах будут пользователи с наибольшим счетом);
- `true` — по возрастанию (на первых местах будут пользователи с наименьшим счетом).

[*key_sort_order]: Направление сортировки в строковом формате:
- `'DESC'` — по убыванию (на первых местах будут пользователи с наибольшим счетом);
- `'ASC'` — по возрастанию (на первых местах будут пользователи с наименьшим счетом).

[*key_decimal_offset]: Размер десятичной части счета. Например, при `decimal_offset: 2` число 1234 будет отображаться как 12.34.

[*key_type]: Тип результата лидерборда. Доступные значения: `numeric` (число), `time` (миллисекунды).

[*key_title]: Список локализованных названий. Возможные коды языков перечислены на странице [{#T}](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains.md).

[*key_score]: Значение результата. Не может быть отрицательным, максимальное значение ограничено только логикой JavaScript.

[*key_extraData]: Описание пользователя.

[*key_userRank]: Позиция пользователя в лидерборде.

[*key_publicName]: Имя пользователя.

[*key_uniqueID]: Уникальный идентификатор пользователя.

[*key_getAvatarSrc]: Возвращает URL портрета пользователя. Возможные значения `size`: `small`, `medium`, `large`.

[*key_getAvatarSrcSet]: Возвращает srcset портрета пользователя, который подходит для дисплеев Retina. Возможные значения `size`: `small`, `medium`, `large`.

[*key_includeUser]: Определяет, включать ли авторизованного пользователя в ответ:
- `true` — включать в ответ.
- `false` (по умолчанию) — не включать.

[*key_quantityAround]: Количество записей ниже и выше пользователя по лидерборду, которое нужно вернуть. Минимальное значение — 1, максимальное — 10. По умолчанию возвращается 5.

[*key_quantityTop]: Количество записей из топа лидерборда. Минимальное значение — 1, максимальное — 20. По умолчанию возвращается 5.

[*key_leaderboard]: [{#T}](#description).

[*key_ranges]: Интервалы мест в ответе.

[*key_start]: Место в рейтинге. Счет ведется с нуля, поэтому 1-е место считается нулевым элементом.

[*key_size]: Количество запрошенных записей. Если данных не хватает, то может не соответствовать ответу.

[*key_entries]: Массив записей рейтинга. Запись идентична возвращаемому значению из метода [{#T}](#get-entry).