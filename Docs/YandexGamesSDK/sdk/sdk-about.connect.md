# Подключение `/sdk.js` (восстановленные примеры)

В снимке [`sdk-about.md`](sdk-about.md) теги `<script>` вырезаны санитизацией Diplodoc/HTML. Ниже — актуальные примеры с официальной страницы [Подключение и использование](https://yandex.ru/dev/games/doc/ru/sdk/sdk-about.md) и из шаблона этой игры. Источник: 2026-08-18.

Скрипт **обязан** загрузиться до `YaGames.init()`. На серверах Яндекса (архив через консоль) — только относительный путь.

## Относительный путь (рекомендуется) {#yandex-server}

Игра в архиве на хостинге Яндекс Игр:

```html
<script src="/sdk.js"></script>
```

Так уже сделано в `Assets/WebGLTemplates/YandexGames/index.html`.

С `async` и колбэком после загрузки:

```html
<!-- Yandex Games SDK -->
<script src="/sdk.js" async onload="initSDK()"></script>
```

`initSDK` — ваша функция, внутри которой `await YaGames.init()`.

## Абсолютный путь (свой домен / iframe) {#iframe}

```html
<script src="https://sdk.games.s3.yandex.net/sdk.js"></script>
```

Для публикации в каталоге Яндекса этот вариант **не** нужен.

## Динамическая загрузка

```html
<script>
(function (d) {
    var t = d.getElementsByTagName('script')[0];
    var s = d.createElement('script');
    s.src = '/sdk.js';
    s.async = true;
    s.onload = initSDK;
    t.parentNode.insertBefore(s, t);
})(document);
</script>
```

## Инициализация

Клиент (эта игра — без своего платёжного сервера):

```javascript
const ysdk = await YaGames.init();
```

Серверная проверка подписи покупок (не используется):

```javascript
const ysdk = await YaGames.init({ signed: true });
```

Проверка на debug-панели: индикатор лоадера `IT`. `IF` — старый лоадер, модерация по п. 1.19.1 не пройдёт. `W` — `init()` ещё не вызван.
