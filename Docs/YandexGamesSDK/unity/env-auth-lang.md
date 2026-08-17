---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/unity/env-auth-lang.md
  - https://yandex.ru/dev/games/doc/hi/sdk/unity/env-auth-lang.md
  - https://yandex.ru/dev/games/doc/ko/sdk/unity/env-auth-lang.md
  - https://yandex.ru/dev/games/doc/ru/sdk/unity/env-auth-lang.md
  - https://yandex.ru/dev/games/doc/tr/sdk/unity/env-auth-lang.md
  - https://yandex.ru/dev/games/doc/vi/sdk/unity/env-auth-lang.md
  - https://yandex.ru/dev/games/doc/zh/sdk/unity/env-auth-lang.md
  - href: ru/sdk/unity/env-auth-lang.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Переменные окружения, авторизация, локализация

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

#|
|| ![SVG](../../_images/icons/video.svg) | Видеоурок 3. Получение данных, авторизация и локализация

<div class="cut-button">

{% cut "Посмотреть видео" %}


{% list tabs %}

- Яндекс

  @[](https://runtime.strm.yandex.ru/player/video/vplviwz6uirn3yuha6kz?autoplay=0&mute=0)

- YouTube

  @[youtube](https://youtu.be/olyTBkJJAfY?si=0ja5BNd6PE805aOp)

{% endlist %}

{% endcut %}

</div>

||
|#

Функции плагина разделены на отдельные модули (ссылки даны на инструкции к ним):
- [Переменные окружения](https://max-games.ru/plugin-yg/doc/envir/){.external} дают возможность получить от SDK платформы данные об устройстве пользователя, такие как язык, операционная система или браузер.
- [Авторизация](https://max-games.ru/plugin-yg/doc/auth/){.external} позволяет подключить лидерборды и проводить платежи. Однако игра должна быть доступна и без авторизации (см. [пункт 1.2](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-2)).
  С помощью модуля авторизации можно получить данные пользователя, например, его ник или аватар. Также он необходим для некоторых модулей, таких как Leaderboards или Payments.
- [Языки](https://max-games.ru/plugin-yg/doc/lang/){.external}:
    - [Управление локализациями игры](https://max-games.ru/plugin-yg/doc/lang/){.external}: при определении языка платформы язык игры меняется на соответствующий.

      {% note warning %}

      Игра должна быть переведена хотя бы на один выбранный при заполнении черновика язык (см. [пункт 1.2](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#2-10)). Рекомендуется добавить [локализации](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains.md#languages) для ru, en, tr.

      Если язык в игре выбирается вручную, обратите внимание на [пункт 6.9](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#6-9) Требований к игре.

      {% endnote %}

    - [Автоматический перевод](https://max-games.ru/plugin-yg/doc/lang/#Auto_Translate_Langs){.external} с помощью Google Переводчика.


---

<!-- source: ru/_includes/plugin-tg-support.md -->
{% note info %}

В [телеграм-канале](https://t.me/pluginYG2){.telegram} по PluginYG2 публикуются обновления плагина и его модулей и полезная информация. По вопросам пишите в [чат плагина](https://t.me/pluginYG){.telegram}.

{% endnote %}
<!-- endsource: ru/_includes/plugin-tg-support.md -->

