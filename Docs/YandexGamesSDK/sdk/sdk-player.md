---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-player.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-player.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-player.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-player.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-player.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-player.md
  - href: ru/sdk/sdk-player.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Данные игрока

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

Вы можете:

- Сохранять данные игры (пройденные уровни, опыт, инап-покупки и т. д.) на сервере Яндекса с помощью методов SDK или передавать их на свой сервер. Облачные сохранения позволяют пользователям продолжать игру на разных устройствах.
- Персонализировать игру с помощью данных из профиля пользователя на Яндексе, например имени.

Для работы с данными пользователя используется объект `Player`.

## Инициализация {#getplayer}

Чтобы инициализировать объект `Player`, используйте метод `ysdk.getPlayer()`:

```javascript showLineNumbers
const ysdk = await YaGames.init();

try {
    const player = await ysdk.getPlayer();
} catch (e) {
    // Ошибка при инициализации объекта Player.
}
```

При инициализации объекта `Player` передаются:

- Идентификатор пользователя — для всех.
- Аватар и имя — для авторизованных игроков.
- Данные о покупках на платформе (только для игр с [покупками](https://yandex.ru/dev/games/doc/ru/console/purchases.md)) — для игроков из РФ.

Подробнее об этих параметрах читайте в разделе [Данные профиля пользователя](#profile-data).

Доступ к данным пользователя зависит от настроек в его [профиле](https://yandex.ru/games/user){.external}. Если игрок запретил доступ к персональным данным, в ответе будет только идентификатор.

Чтобы авторизовать пользователя и сохранять данные состояния игры на своем сервере, используйте опциональный параметр `{ signed: true }` и метод `fetch()`. Это позволит вам проверять подлинность игрока с помощью [секретного ключа](https://yandex.ru/dev/games/doc/ru/sdk/sdk-purchases.md#key-example) и избежать возможных накруток. Ключ становится доступен после [подключения инап-покупок](https://yandex.ru/dev/games/doc/ru/console/purchases.md#connect).


```javascript showLineNumbers
const ysdk = await YaGames.init();

try {
    const player = await ysdk.getPlayer({ signed: true });

    // Используйте player.signature для авторизации на своем сервере.
    const authData = await fetch('https://your.game.server/auth', {
        method: 'POST',
        headers: { 'Content-Type': 'text/plain' },
        body: player.signature
    });
} catch (e) {
    // Ошибка при инициализации объекта Player или авторизации.
}
```

Параметр `signature` передаваемого на сервер запроса содержит данные пользователя из профиля на Яндексе и подпись. Представляет собой две строки в кодировке `base64`:

```text
<подпись>.<данные профиля>
```

Подробнее см. в разделе [Защита от накруток](https://yandex.ru/dev/games/doc/ru/sdk/sdk-purchases.md#signature).

{% note info %}

Запрос можно отправлять не чаще, чем 20 раз за 5 минут, иначе он будет отклонен с ошибкой.

{% endnote %}


## Авторизация пользователя {#auth}

### Проверка авторизации {#auth-check}

Чтобы проверить, авторизован ли игрок на Яндексе, используйте метод объекта `Player` — `player.isAuthorized()`. Метод возвращает `true | false`.

{% note alert %}

Метод `player.getMode(): 'lite' | ''` устарел и позже будет удален из интерфейса.

{% endnote %}


### Вызов диалогового окна авторизации {#open-auth-dialog}

Чтобы вызвать окно авторизации, используйте метод `ysdk.auth.openAuthDialog()`.

{% note warning %}

Проинформируйте пользователя о преимуществах, которые дает авторизация. Если пользователь не будет понимать, зачем она нужна, то с большой вероятностью он откажется от авторизации и выйдет из игры.

Подробнее см. в разделе [Предложение авторизации](https://yandex.ru/dev/games/doc/ru/requirements/1/2.md#auth-offer).

{% endnote %}


```javascript showLineNumbers
const ysdk = await YaGames.init();

try {
    let player = await ysdk.getPlayer();

    // Игрок не авторизован.
    if (!player.isAuthorized()) {
        try {
            // Открытие окна авторизации.
            await ysdk.auth.openAuthDialog();

            const authorizedPlayer = await ysdk.getPlayer();

            player = authorizedPlayer;
        } catch (e) {
            // Ошибка при авторизации игрока или повторной инициализации объекта Player.
        }
    }
    // Игрок успешно авторизован.
} catch (err) {
    // Ошибка при инициализации объекта Player.
}
```

## Внутриигровые данные {#ingame-data}

Для работы с внутриигровыми данными пользователя используйте методы объекта `Player`.

### player.setData(data, flush) {#setdata}

Сохраняет данные пользователя. Максимальный размер данных на игрока — 200&nbsp;KБ.

**Сигнатура метода**

```typescript
function setData(data: object, flush: boolean) => Promise<void> {}
```

Принимает параметры:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
||`data` | `object` | Объект, содержащий пары «ключ — значение». ||
|| `flush` | `boolean` | Определяет очередность отправки данных:
- `true` — данные будут отправлены на сервер немедленно.
- `false` (значение по умолчанию) — запрос на отправку данных будет поставлен в очередь. ||
|#

</div>

Метод возвращает `Promise`, который показывает, удалось сохранить данные или нет.

При значении параметра `flush: false` возвращаемый результат показывает только валидность данных (сама отправка поставлена в очередь и будет осуществлена позже). При этом метод `player.getData()` вернет данные, установленные последним вызовом `player.setData()`, даже если они еще не были отправлены.

{% note info %}

Запрос можно отправлять не чаще, чем 100 раз за 5 минут, иначе он будет отклонен с ошибкой.

{% endnote %}

#### Пример {#example-setdata}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const player = await ysdk.getPlayer();

await player.setData({
    achievements: ['trophy1', 'trophy2', 'trophy3'],
})

console.log('data is set');
```

### player.getData(keys) {#getdata}

Асинхронно возвращает внутриигровые данные пользователя, сохраненные в базе данных Яндекса.

**Сигнатура метода**

```typescript
function getData(keys?: Array<string>) => Promise<object> {}
```

Принимает параметр:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `keys` | `Array<string>` | Список ключей, которые необходимо вернуть. Если параметр `keys` отсутствует, то метод возвращает все внутриигровые данные пользователя. ||
|#

</div>

Метод возвращает `Promise<object>`, который содержит пары «ключ — значение».

{% note info %}

Запрос можно отправлять не чаще, чем 100 раз за 5 минут, иначе он будет отклонен с ошибкой.

{% endnote %}

### player.setStats(stats) {#setstats}

Сохраняет численные данные пользователя. Максимальный размер численных данных на игрока — 10&nbsp;КБ.

{% note tip %}

Используйте этот метод для часто изменяемых числовых значений (баллы, очки опыта, внутриигровая валюта) вместо [player.setData()](#ingame-data).

{% endnote %}

**Сигнатура метода**

```typescript
function setStats(stats?: object) => Promise<void> {}
```

Принимает параметр:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `stats` | `object` | Объект, содержащий пары «ключ — значение», где каждое значение должно быть числом. ||
|#

</div>

Метод возвращает `Promise`, который показывает, удалось сохранить данные или нет.

{% note info %}

Запрос можно отправлять не чаще, чем 60 раз за 1 минуту, иначе он будет отклонен с ошибкой.

{% endnote %}

### player.incrementStats(increments) {#incrementstats}

Изменяет численные данные пользователя. Максимальный размер численных данных на игрока — 10&nbsp;КБ.

**Сигнатура метода**

```typescript
function incrementStats(increments: object) => Promise<object> {}
```

Принимает параметр:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `increments` | `object` | Объект, который содержит пары «ключ — значение», где каждое значение должно быть числом. ||
|#

</div>

Метод возвращает `Promise<object>`, который содержит измененные и добавленные пары «ключ — значение»

{% note info %}

Запрос можно отправлять не чаще, чем 60 раз за 1 минуту, иначе он будет отклонен с ошибкой.

{% endnote %}

### player.getStats(keys) {#getstats}

Асинхронно возвращает численные данные пользователя.

**Сигнатура метода**

```typescript
function getStats(keys?: Array<string>) => Promise<object> {}
```

Принимает параметр:

<div class="table-25 table-2c25">

#|
|| **Параметр** | **Тип** | **Описание** ||
|| `keys` | `Array<string>` | Cписок ключей, которые необходимо вернуть. Если параметр `keys` отсутствует, то метод возвращает все численные данные пользователя. ||
|#

</div>

Метод возвращает `Promise<object>`, который содержит пары «ключ — значение».

{% note info %}

Запрос можно отправлять не чаще, чем 60 раз за 1 минуту, иначе он будет отклонен с ошибкой.

{% endnote %}


## Данные профиля пользователя {#profile-data}

Чтобы получить данные из профиля пользователя на Яндексе, используйте методы объекта `Player`.

### player.getUniqueID() {#getuniqueid}

Возвращает постоянный уникальный идентификатор пользователя.

**Сигнатура метода**

```typescript
function getUniqueID() => string {}
```

{% note info %}

Метод `player.getID()` устарел, но пока продолжит работать с предупреждением в консоли ошибок.

Значения `player.getID()` и `player.getUniqueID()` в общем случае не совпадают для одного объекта `Player`, но для некоторых пользователей могут быть одинаковыми. Если значения различаются и игра ранее самостоятельно привязывала к значению `player.getID()` какие-либо данные, выполните миграцию этих данных, привязав их к значению `player.getUniqueID()`. Чтобы выполнить миграцию сразу для всех пользователей, напишите в [службу поддержки](https://yandex.ru/dev/games/doc/ru/concepts/troubleshooting.md).

{% endnote %}


### player.getIDsPerGame() {#getidspergame}

{% note alert %}

Запрос доступен только для авторизованных пользователей. Как проверить статус авторизации и вызвать диалог входа, см. в разделе [Авторизация пользователя](#auth).

Перед отправкой запроса проверьте доступность метода с помощью `ysdk.isAvailableMethod('player.getIDsPerGame')`. Метод возвращает `Promise<Boolean>`.

{% endnote %}

Метод возвращает массив объектов с указанием идентификаторов пользователя во всех играх разработчика, в которых пользователь явно дал согласие на передачу персональных данных.

**Сигнатура метода**

```typescript
function getIDsPerGame() => Promise<Array<{ appID: number, userID: string }>> {}
```

### player.getName() {#getname}

Возвращает имя пользователя.

**Сигнатура метода**

```typescript
function getName() => string {}
```

### player.getPhoto() {#getphoto}

Возвращает URL аватара пользователя в зависимости от размера запрашиваемого изображения.

**Сигнатура метода**

```typescript
function getPhoto(size: 'small' | 'medium' | 'large') => string {}
```

### player.getPayingStatus() {#getpayingstatus}

Возвращает значение, зависящее от частоты и объема покупок пользователя.

**Сигнатура метода**

```typescript
function getPayingStatus() => EPayingStatus {}
```

`EPayingStatus` принимает одно из значений:

#|
|| **Значение** | **Описание** ||
|| `paying` | Пользователь купил портальную валюту на сумму более 500&nbsp;рублей за последний месяц. ||
|| `partially_paying` | У пользователя была хотя бы одна покупка портальной валюты реальными деньгами за последний год. ||
|| `not_paying` | Пользователь не делал покупок портальной валюты реальными деньгами за последний год. ||
|| `unknown` | Пользователь не из РФ или он не разрешил передачу такой информации разработчику. ||
|#

#### Пример {#example-status}

```javascript showLineNumbers
const ysdk = await YaGames.init(); // Инициализируем SDK.
const player = await ysdk.getPlayer(); // Получаем игрока.
const payingStatus = player.getPayingStatus(); // Получаем статус платежной активности пользователя на платформе.

if (payingStatus === 'paying' || payingStatus === 'partially_paying') {
    // Предложить инап-товар на старте или вместо рекламы.
}
```

## Ограничения методов {#limits}

#|
|| **Метод** | **Описание** | **Лимит** ||
|| `ysdk.getPlayer()` | [Инициализирует объект `Player`](#getplayer) |::{align="center"} 20 запросов за 5 минут ||
|| `player.setData()` | [Сохраняет данные пользователя](#setdata) |::{align="center"} 100 запросов за 5 минут ||
|| `player.getData()` | [Асинхронно возвращает внутриигровые данные пользователя](#getdata) | ^ ||
|| `player.setStats()` | [Сохраняет численные данные пользователя](#setstats) |::{align="center"} 60 запросов за 1 минуту ||
|| `player.getStats()` | [Асинхронно возвращает численные данные пользователя](#getstats) | ^ ||
|| `player.incrementStats()` | [Изменяет численные данные пользователя](#incrementstats) | ^ ||
|#

## Потеря прогресса на iOS {#progress-loss}

Если для интеграции игры вы используете свой домен, на новых версиях iOS хранилище `localStorage` может часто сбрасываться, из-за чего игроки теряют прогресс. Чтобы этого избежать, используйте хранилище `safeStorage`, у которого такой же интерфейс, как у `localStorage`:

```javascript showLineNumbers
const ysdk = await YaGames.init();

const safeStorage = await ysdk.getStorage();

safeStorage.setItem('key', 'safe storage is working');
console.log(safeStorage.getItem('key'));
```

Чтобы не менять код вручную, переопределите `localStorage` глобально.

{% note alert %}

Убедитесь, что `localStorage` не используется до переопределения.

{% endnote %}


```javascript showLineNumbers
const ysdk = await YaGames.init();

const safeStorage = await ysdk.getStorage();

Object.defineProperty(window, 'localStorage', { get: () => safeStorage });

localStorage.setItem('key', 'safe storage is working');
console.log(localStorage.getItem('key'));
```

Если вы загружаете исходный код в виде архива, ничего делать не нужно: специальная обертка в SDK автоматически делает `localStorage` надежным.

## Решение проблем {#faq}

#### Что делать, если размер сохранений превышает лимиты SDK? {#large-saves}

Методы SDK имеют ограничения на максимальный размер данных на игрока:

#|
|| **Метод** | **Описание** | **Лимит** ||
|| `player.setData()` | [Данные пользователя](#setdata) | 200 КБ ||
|| `player.setStats()` | [Статистика (численные значения)](#setstats) | 10 КБ ||
|#

Если вашей игре требуется сохранять больше данных (например, в стратегиях с большим количеством юнитов или сложным состоянием мира), используйте собственный сервер для хранения прогресса. Подробнее о способах хранения данных см. в разделе [Где сохранять прогресс](https://yandex.ru/dev/games/doc/ru/requirements/1/9.md#save-location).

#### Как сбросить прогресс игрока через код? {#reset-progress}

Чтобы очистить данные игрока, запишите пустой прогресс с помощью методов [player.setData()](#setdata) и [player.setStats()](#setstats).

Для тестирования сброса прогресса вы также можете использовать кнопку ☁️ **Clear cloud data** на [debug-панели](https://yandex.ru/dev/games/doc/ru/console/debug-panel.md#cloud-icon).


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
