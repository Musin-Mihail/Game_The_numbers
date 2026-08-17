---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/unity/advertisement.md
  - https://yandex.ru/dev/games/doc/hi/sdk/unity/advertisement.md
  - https://yandex.ru/dev/games/doc/ko/sdk/unity/advertisement.md
  - https://yandex.ru/dev/games/doc/ru/sdk/unity/advertisement.md
  - https://yandex.ru/dev/games/doc/tr/sdk/unity/advertisement.md
  - https://yandex.ru/dev/games/doc/vi/sdk/unity/advertisement.md
  - https://yandex.ru/dev/games/doc/zh/sdk/unity/advertisement.md
  - href: ru/sdk/unity/advertisement.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Реклама

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
|| ![SVG](../../_images/icons/video.svg) | Видеоурок 2. Реклама в играх

<div class="cut-button">

{% cut "Посмотреть видео" %}


{% list tabs %}

- Яндекс

  @[](https://runtime.strm.yandex.ru/player/video/vplvlc4zzcshcp4ztpey?autoplay=0&mute=0)

- YouTube

  @[youtube](https://youtu.be/B9J2G2hoXKA?si=ky4V0HOrU45p-283)

{% endlist %}

{% endcut %}

</div>

||
|#

Подробнее о [рекламной монетизации](https://yandex.ru/dev/games/doc/ru/console/adv-monetization.md).

Плагин позволяет работать с тремя видами рекламы:
- [Полноэкранная реклама](https://yandex.ru/dev/games/doc/ru/console/adv-monetization.md#interstitial), которая показывается в логических паузах. [Подробнее](https://max-games.ru/plugin-yg/doc/inter-ad/){.external}.
- [Видео за вознаграждение](https://yandex.ru/dev/games/doc/ru/console/adv-monetization.md#rewarded). В этом случае пользователю должно быть понятно, что ему предлагают посмотреть рекламу и что он получит взамен. [Подробнее](https://max-games.ru/plugin-yg/doc/reward-ad/){.external}.
- [Стики-баннеры](https://yandex.ru/dev/games/doc/ru/console/adv-monetization.md#sticky), они отображаются по краям экрана в течение игры. [Подробнее](https://max-games.ru/plugin-yg/doc/sticky-adv/){.external}.

Подключение рекламной монетизации обязательно ([пункт 1.12](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-12)), однако нельзя подключать стороннюю рекламу ([пункт 1.16](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-16)). Показ рекламы регламентирован [разделом 4](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#4) Требований к игре. После настройки рекламы вы можете протестировать ее с помощью плагина.


---

<!-- source: ru/_includes/plugin-tg-support.md -->
{% note info %}

В [телеграм-канале](https://t.me/pluginYG2){.telegram} по PluginYG2 публикуются обновления плагина и его модулей и полезная информация. По вопросам пишите в [чат плагина](https://t.me/pluginYG){.telegram}.

{% endnote %}
<!-- endsource: ru/_includes/plugin-tg-support.md -->

