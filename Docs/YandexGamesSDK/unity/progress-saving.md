---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/unity/progress-saving.md
  - https://yandex.ru/dev/games/doc/hi/sdk/unity/progress-saving.md
  - https://yandex.ru/dev/games/doc/ko/sdk/unity/progress-saving.md
  - https://yandex.ru/dev/games/doc/ru/sdk/unity/progress-saving.md
  - https://yandex.ru/dev/games/doc/tr/sdk/unity/progress-saving.md
  - https://yandex.ru/dev/games/doc/vi/sdk/unity/progress-saving.md
  - https://yandex.ru/dev/games/doc/zh/sdk/unity/progress-saving.md
  - href: ru/sdk/unity/progress-saving.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Облачные сохранения и работа с данными

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
|| ![SVG](../../_images/icons/video.svg) | Видеоурок 4. Облачные сохранения и работа с данными

<div class="cut-button">

{% cut "Посмотреть видео" %}


{% list tabs %}

- Яндекс

  @[](https://runtime.strm.yandex.ru/player/video/vplv7lqkgm4gplipbdge?autoplay=0&mute=0)

- YouTube

  @[youtube](https://youtu.be/QCgXHR6sBIk?si=dxZzUJ6ZGzwnLFJ-)

{% endlist %}

{% endcut %}

</div>

||
|#

[Сохранение прогресса](https://max-games.ru/plugin-yg/doc/storage/){.external} позволяет создать комфортный и разнообразный игровой опыт. Вы можете сохранять данные о состоянии игры (пройденные уровни, опыт, инап-покупки и т. д.) на сервере Яндекса или передавать их на свой сервер. Также вы можете персонализировать игру, используя данные из профиля пользователя, например, имя.

В некоторых случаях сохранение прогресса в игре необходимо:
- У игрока должна быть возможность продолжить игру после обновления ее страницы, не теряя свои достижения или рекорды (см. [пояснение](https://yandex.ru/dev/games/doc/ru/requirements/1/9.md) к пункту 1.9).
- Если в игре предполагается:
  - Движение по сюжету.
  - Постепенное прохождение уровней.
  - Постановка рекордов.
- Если игра бесконечная (см. [пункт 2.6](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#2-6)).

Если вы используете облачные сохранения, укажите это в черновике (см. [пункт 1.11](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-11)).


---

<!-- source: ru/_includes/plugin-tg-support.md -->
{% note info %}

В [телеграм-канале](https://t.me/pluginYG2){.telegram} по PluginYG2 публикуются обновления плагина и его модулей и полезная информация. По вопросам пишите в [чат плагина](https://t.me/pluginYG){.telegram}.

{% endnote %}
<!-- endsource: ru/_includes/plugin-tg-support.md -->

