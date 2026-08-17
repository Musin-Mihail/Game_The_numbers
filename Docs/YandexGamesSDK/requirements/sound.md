---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/requirements/1/3.md
  - https://yandex.ru/dev/games/doc/hi/requirements/1/3.md
  - https://yandex.ru/dev/games/doc/ko/requirements/1/3.md
  - https://yandex.ru/dev/games/doc/ru/requirements/1/3.md
  - https://yandex.ru/dev/games/doc/tr/requirements/1/3.md
  - https://yandex.ru/dev/games/doc/vi/requirements/1/3.md
  - https://yandex.ru/dev/games/doc/zh/requirements/1/3.md
  - href: ru/requirements/1/3.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Звук вне игры

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

{% note info "Пункт 1.3 Требований к игре" %}

При сворачивании страницы с игрой на десктопных и мобильных устройствах звук останавливается.

{% endnote %}

## Методика проверки {#verification-method}

Модерация проверяет, останавливается ли звук из игры:
 - При сворачивании окна браузера на компьютере или приложения на телефоне.
 - После переключения на другую вкладку в том же окне браузера.
 - В браузере в [меню выбора вкладок](*menu).

Если во всех трех случаях звук останавливается, то игра будет одобрена. При этом не считается нарушением, если:
 - Звук останавливается не сразу, а в течение 2 секунд после выхода из вкладки с игрой.
 - Звук из игры не останавливается после клика на баннер с рекламой и перехода на вкладку с предложением.
 - На iOS в [меню выбора вкладок](*menu) звук из игры продолжает воспроизводиться.

   {% cut "Пример звука из игры на iOS" %}

   @[](https://runtime.strm.yandex.ru/player/video/vplvqpjri3mpf4ej2g2d?autoplay=0&mute=0)

   {% endcut %}

[*menu]: ![Скриншот окна браузера с выбором вкладок.](../../_images/requirements/1-3-popup/choosing.webp){width=300}