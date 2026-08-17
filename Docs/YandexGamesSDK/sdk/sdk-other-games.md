---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-other-games.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-other-games.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-other-games.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-other-games.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-other-games.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-other-games.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-other-games.md
  - href: ru/sdk/sdk-other-games.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Ссылки на другие игры

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

SDK Яндекс Игр дает возможность автоматически получить корректные ссылки на другие ваши игры (как на конкретные, так и на весь список), чтобы сослаться на них в игре.

Чтобы игра точно была доступна на текущих платформе и домене ([пункт 8.4.1](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#8-4-1)), используйте методы `GamesAPI.getAllGames()` и `GamesAPI.getGameByID()`.

## ysdk.features.GamesAPI.getAllGames() {#get-all-games}

Используйте метод, когда вам нужно получить информацию обо всех своих играх, которые доступны на текущих платформе и домене.

### Пример {#get-all-games-ex}

```javascript showLineNumbers
ysdk.features.GamesAPI.getAllGames().then(({games, developerURL}) => {
    games.forEach((game) => {
        // Логика обработки игры.
    })
}).catch(err => {
    // Ошибка при получении данных об игре.
})
```

### Формат ответа {#get-all-games-response}

```javascript showLineNumbers
{
    [games](*key_games): IGame[];
    [developerURL](*key_developerURL): string;
}
```

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `games` | [IGame[]](#igame) | Массив объектов с информацией об играх. ||
|| `developerURL` | `string` | Ссылка на страницу разработчика. ||
|#

## ysdk.features.GamesAPI.getGameByID() {#get-game-by-id}

Используйте метод, когда вам нужно получить данные о конкретной игре и ее доступности на текущих платформе и домене.

Принимает параметр:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `appID` | `number` | ID игры из Консоли разработчика. ||
|#

</div>

### Пример {#get-by-id-ex}

```javascript showLineNumbers
ysdk.features.GamesAPI.getGameByID(100000).then(({isAvailable, game}) => {
    if (isAvailable) {
        // Если игра доступна, обработайте game.
    } else {
        // Логика, если игра недоступна, объект game не определен (undefined).
    }
}).catch(err => {
    // Ошибка при получении данных об игре.
})
```

### Формат ответа {#get-by-id-response}

```javascript showLineNumbers
{
    [game](*key_game)?: IGame;
    [isAvailable](*key_isAvailable): boolean;
}
```

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `game` | [IGame](#igame) | Объект с информацией об игре. ||
|| `isAvailable` | `boolean` | Показывает, доступна ли игра:
* `true` — игра доступна;
* `false` — игра недоступна, объект `game` не определен (`undefined`). ||
|#

## Интерфейс IGame {#igame}

```typescript showLineNumbers
interface IGame {
    [appID](*key_appId): string;
    [title](*key_title): string;
    [url](*key_url): string;
    [coverURL](*key_coverURL): string;
    [iconURL](*key_iconURL): string;
}
```

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `appID` | `string` | Идентификатор игры, который задан в Консоли разработчика. ||
|| `title` | `string` | Название игры. ||
|| `url` | `string` | Ссылка на игру. ||
|| `coverURL` | `string` | Ссылка на обложку игры. ||
|| `iconURL` | `string` | Ссылка на иконку игры. ||
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

[*key_games]: Массив объектов с информацией об играх.

[*key_developerURL]: Ссылка на страницу разработчика.

[*key_game]: Объект с информацией об игре.

[*key_isAvailable]: Показывает, доступна ли игра:
* `true` — игра доступна;
* `false` — игра недоступна, объект `game` не определен (`undefined`).

[*key_appId]: Идентификатор игры, который задан в Консоли разработчика.

[*key_title]: Название игры.

[*key_url]: Ссылка на игру.

[*key_coverURL]: Ссылка на обложку игры.

[*key_iconURL]: Ссылка на иконку игры.