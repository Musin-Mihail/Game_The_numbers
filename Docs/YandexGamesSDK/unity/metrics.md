---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/unity/metrics.md
  - https://yandex.ru/dev/games/doc/hi/sdk/unity/metrics.md
  - https://yandex.ru/dev/games/doc/ko/sdk/unity/metrics.md
  - https://yandex.ru/dev/games/doc/ru/sdk/unity/metrics.md
  - https://yandex.ru/dev/games/doc/tr/sdk/unity/metrics.md
  - https://yandex.ru/dev/games/doc/vi/sdk/unity/metrics.md
  - https://yandex.ru/dev/games/doc/zh/sdk/unity/metrics.md
  - href: ru/sdk/unity/metrics.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Метрики

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
|| ![SVG](../../_images/icons/video.svg) | Видеоурок 7. Метрики в Яндекс Играх

<div class="cut-button">

{% cut "Посмотреть видео" %}


{% list tabs %}

- Яндекс

  @[](https://runtime.strm.yandex.ru/player/video/vplvsuch6vppvbewrqjo?autoplay=0&mute=0)

- YouTube

  @[youtube](https://youtu.be/XdYtMTFk7jg?si=xmUa0wEWQsNQf8Uw)

{% endlist %}

{% endcut %}

</div>

||
|#

[Метрики](https://yandex.ru/dev/games/doc/ru/concepts/metric.md) — это показатели, которые позволяют отслеживать ключевые события. Например, количество показов рекламы или инап-покупок в день. Изучив показатели игры до и после внесения изменений, можно узнать, как они влияют на поведение пользователей: какие механики работают хорошо, а какие требуют улучшения.

Метрики монетизации, конверсии и некоторые другие метрики отображаются в [Консоли Яндекс Игр](https://games.yandex.ru/console){.external}. Для отслеживания других событий и показателей создайте счетчик, например, в Яндекс Метрике и добавьте его в игру с помощью [плагина](https://max-games.ru/plugin-yg/doc/metrica/){.external}.


---

<!-- source: ru/_includes/plugin-tg-support.md -->
{% note info %}

В [телеграм-канале](https://t.me/pluginYG2){.telegram} по PluginYG2 публикуются обновления плагина и его модулей и полезная информация. По вопросам пишите в [чат плагина](https://t.me/pluginYG){.telegram}.

{% endnote %}
<!-- endsource: ru/_includes/plugin-tg-support.md -->

