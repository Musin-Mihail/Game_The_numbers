---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/unity/other.md
  - https://yandex.ru/dev/games/doc/hi/sdk/unity/other.md
  - https://yandex.ru/dev/games/doc/ko/sdk/unity/other.md
  - https://yandex.ru/dev/games/doc/ru/sdk/unity/other.md
  - https://yandex.ru/dev/games/doc/tr/sdk/unity/other.md
  - https://yandex.ru/dev/games/doc/vi/sdk/unity/other.md
  - https://yandex.ru/dev/games/doc/zh/sdk/unity/other.md
  - href: ru/sdk/unity/other.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Серверное время, награда за отзыв и ярлык на рабочем столе

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
|| ![SVG](../../_images/icons/video.svg) | Видеоурок 8. Серверное время и награда за отзыв и ярлык на рабочем столе

<div class="cut-button">

{% cut "Посмотреть видео" %}


{% list tabs %}

- Яндекс

  @[](https://runtime.strm.yandex.ru/player/video/vplv7xfyz76ujqgqtb5o?autoplay=0&mute=0)

- YouTube

  @[youtube](https://youtu.be/JxiatIGGZq0?si=sUljBIqOgKJbnFm4)

{% endlist %}

{% endcut %}

</div>

||
|#

[Серверное время](https://max-games.ru/plugin-yg/doc/server-time/){.external} полезно для:
- **Защиты от накруток**: пользователи не смогут влиять на игровые процессы, изменяя время на своем устройстве.
- **Игровых событий**: на его базе вы можете добавлять активности и награды, для которых важен доверенный источник времени. Например, ежедневные или еженедельные бонусы, сезонные события и квесты.

[Оценка игры](https://max-games.ru/plugin-yg/doc/review/){.external}: вы можете попросить пользователя оценить игру или написать комментарий. Запрос увидят только авторизованные пользователи, которые не оценивали игру ранее. Перед отправкой запроса проверьте, что у пользователя есть возможность оставить оценку.

[Ярлык на рабочий стол](https://max-games.ru/plugin-yg/doc/game-label/){.external}: вы можете предложить пользователю добавить на рабочий стол ярлык (ссылку на игру) через нативное диалоговое окно. Но прежде чем вывести диалоговое окно, убедитесь, что опция доступна.


---

<!-- source: ru/_includes/plugin-tg-support.md -->
{% note info %}

В [телеграм-канале](https://t.me/pluginYG2){.telegram} по PluginYG2 публикуются обновления плагина и его модулей и полезная информация. По вопросам пишите в [чат плагина](https://t.me/pluginYG){.telegram}.

{% endnote %}
<!-- endsource: ru/_includes/plugin-tg-support.md -->

