---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-params.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-params.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-params.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-params.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-params.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-params.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-params.md
  - href: ru/sdk/sdk-params.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Другие объекты и параметры SDK

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


## Объект screen.fullscreen {#screen-fullscreen}

Предназначен для работы с полноэкранным режимом браузера.


#|
|| **Параметр** | **Тип** | **Описание** ||
|| `STATUS_ON` | `string` | Константа `"on"`. ||
|| `STATUS_OFF` | `string` | Константа `"off"`. ||
|| `status` | `string` | Текущее состояние: `STATUS_ON` или `STATUS_OFF`. ||
|| `request` | `Promise<void>` | Запрос перехода в полноэкранный режим. ||
|| `exit` | `Promise<void>` | Запрос выхода из полноэкранного режима. ||
|#


{% note alert %}

Яндекс Игры могут автоматически запускаться в полноэкранном режиме, однако многие браузеры запрещают переключать режим без команды пользователя.

В правом верхнем углу экрана Яндекс Игр уже реализована кнопка перехода в полноэкранный режим, поэтому используйте параметры объекта `screen.fullscreen` для обработки кнопок непосредственно в игре.

{% endnote %}



## Объект clipboard {#clipboard}

Предназначен для записи строки в буфер обмена при помощи метода `ysdk.clipboard.writeText(text)`.


## Объект deviceInfo {#deviceinfo}

Объект `ysdk.deviceInfo` предназначен для получения информации об устройстве пользователя.

В поле `type` возвращается строка `"desktop"` (компьютер), `"mobile"` (мобильное устройство), `"tablet"` (планшет) или `"tv"` (телевизор), а также все методы с одним из значений.


#|
|| **Метод** | **Описание** ||
|| `isMobile()` | Проверяет устройство пользователя и возвращает значение:
* `true` — мобильное устройство;
* `false` — иное устройство. ||
|| `isDesktop()` | Проверяет устройство пользователя и возвращает значение:
* `true` — компьютер;
* `false` — иное устройство. ||
|| `isTablet()` | Проверяет устройство пользователя и возвращает значение:
* `true` — планшет;
* `false` — иное устройство. ||
|| `isTV()` | Проверяет устройство пользователя и возвращает значение:
* `true` — телевизор;
* `false` — иное устройство. ||
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
