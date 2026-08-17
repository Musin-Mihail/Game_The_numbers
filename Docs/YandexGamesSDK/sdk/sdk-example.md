---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-example.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-example.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-example.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-example.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-example.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-example.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-example.md
  - href: ru/sdk/sdk-example.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Пример

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

Ниже приведены примеры настройки SDK Яндекс Игр при синхронном и асинхронном подключении.

{% list tabs %}

* Синхронное подключение

    Особенности примера:

    - Для первого вызова рекламы [callback-функции](*key_callback) не заданы.
    - Для второго и всех последующих вызовов заданы все возможные callback-функции.
    - Кнопке **Показать рекламу** присвоен обработчик события `'click'` (вызов рекламы при каждом нажатии кнопки).

    ```html showLineNumbers
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
        <meta name="mobile-web-app-capable" content="yes">
        <meta name="apple-mobile-web-app-capable" content="yes">
        <title>Пример страницы с синхронным подключением SDK</title>
        
        
    </head>
    <body>
        <button id="button">Показать рекламу</button>
    </body>
    </html>
    ```

* Асинхронное подключение

    Особенности примера:

    - Для первого вызова рекламы задана callback-функция [onClose](*key_onClose).
    - Для второго и последующих вызовов заданы все возможные [callback-функции](*key_callback).
    - В callback-функцию `onClose` добавлен код, который будет выполняться после закрытия рекламного блока.
    - Все ошибки, возникающие при работе SDK или при выполнении callback-функций, передаются функции [onError](*key_onError).
    - Кнопке **Показать рекламу** присвоен обработчик события `'click'` (вызов рекламы при каждом нажатии кнопки).

    ```html showLineNumbers
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
        <meta name="mobile-web-app-capable" content="yes">
        <meta name="apple-mobile-web-app-capable" content="yes">
        <title>Пример страницы с асинхронным подключением SDK</title>
        
    </head>
    <body>
    <!-- Yandex Games SDK -->
    
    <button id="button">Показать рекламу</button>
    </body>
    </html>
    ```

{% endlist %}

[*key_callback]: - `onClose` — вызывается при закрытии рекламы, после ошибки, а также, если реклама не открылась по причине слишком частого вызова. Используется с аргументом `wasShown` (тип: `boolean`), по значению которого можно узнать была ли показана реклама.
- `onOpen` — вызывается при успешном открытии рекламы.
- `onError` — вызывается при возникновении ошибки. Объект ошибки передается в callback-функцию.

[*key_onClose]: `onClose` — вызывается при закрытии рекламы, после ошибки, а также, если реклама не открылась по причине слишком частого вызова. Используется с аргументом `wasShown` (тип: `boolean`), по значению которого можно узнать была ли показана реклама.

[*key_onError]: `onError` — вызывается при возникновении ошибки. Объект ошибки передается в callback-функцию.

[*key_onOpen]: `onOpen` — вызывается при успешном открытии рекламы.