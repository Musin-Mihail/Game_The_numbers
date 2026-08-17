---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/unity/flags-links.md
  - https://yandex.ru/dev/games/doc/hi/sdk/unity/flags-links.md
  - https://yandex.ru/dev/games/doc/ko/sdk/unity/flags-links.md
  - https://yandex.ru/dev/games/doc/ru/sdk/unity/flags-links.md
  - https://yandex.ru/dev/games/doc/tr/sdk/unity/flags-links.md
  - https://yandex.ru/dev/games/doc/vi/sdk/unity/flags-links.md
  - https://yandex.ru/dev/games/doc/zh/sdk/unity/flags-links.md
  - href: ru/sdk/unity/flags-links.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Флаги, ссылки на другие игры

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
|| ![SVG](../../_images/icons/video.svg) | Видеоурок 9. Удаленная конфигурация игры, ссылки на другие игры

<div class="cut-button">

{% cut "Посмотреть видео" %}


{% list tabs %}

- Яндекс

  @[](https://runtime.strm.yandex.ru/player/video/vplvoqtalwgkjt56frvi?autoplay=0&mute=0)

- YouTube

  @[youtube](https://youtu.be/prpPWD7_Cmo?si=MBm5Y_phoNe4I97I)

{% endlist %}

{% endcut %}

</div>

||
|#

[Флаги](https://max-games.ru/plugin-yg/doc/flags/){.external} позволяют в любое время менять данные игры, что упрощает настройку баланса в игре. Например, можно отредактировать характеристики персонажа или стоимость улучшения, не загружая новый билд. Яндекс Игры позволяют проводить [эксперименты](https://yandex.ru/dev/games/doc/ru/console/vq-test.md): пользователи распределяются на несколько групп, каждой из которых будет показан уникальный экспериментальный вариант приложения.

[Ссылки](https://max-games.ru/plugin-yg/doc/open-url/){.external} на другие игры помогут расширить аудиторию. Интегрировать список доступных игр можно с помощью [префаба](*prefab), а отображение каждой из игр можно контролировать удаленно с помощью флагов.


---

<!-- source: ru/_includes/plugin-tg-support.md -->
{% note info %}

В [телеграм-канале](https://t.me/pluginYG2){.telegram} по PluginYG2 публикуются обновления плагина и его модулей и полезная информация. По вопросам пишите в [чат плагина](https://t.me/pluginYG){.telegram}.

{% endnote %}
<!-- endsource: ru/_includes/plugin-tg-support.md -->

[*prefab]: Prefab — шаблон для объекта в игровом движке Unity.