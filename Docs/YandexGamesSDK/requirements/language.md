---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/requirements/2/14.md
  - https://yandex.ru/dev/games/doc/ru/requirements/2/14.md
  - https://yandex.ru/dev/games/doc/tr/requirements/2/14.md
  - https://yandex.ru/dev/games/doc/zh/requirements/2/14.md
  - href: ru/requirements/2/14.md
    type: text/markdown
    title: Markdown version
  - href: ../../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Автоопределение языка

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

{% note info "Пункт 2.14 Требований к игре" %}

В игру встроено автоопределение языка через SDK.

{% endnote %}



## Подключение {#sdk-i18n}

{% note warning %}

Автоопределение через SDK должно:

- Происходить во время запуска, а не в процессе игры.
- Быть реализовано у всех игр, даже если у них заявлен только один язык или в них нет текстов.

{% endnote %}

Чтобы определить язык пользователя, получите [код языка](*lang_code) из параметра [environment.i18n.lang](https://yandex.ru/dev/games/doc/ru/sdk/sdk-environment.md#structure-i18n):

```javascript showLineNumbers
const ysdk = (await YaGames.init());
const lang = ysdk.environment.i18n.lang; // 'en', 'ru', ...
```

Дальше используйте переменную `lang` при загрузке переводов игры. Если язык пользователя не поддержан в игре, ориентируйтесь на [резервный набор языков](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains.md#languages).



## Методика проверки {#verification-method}

### Проверка одного языка {#one-lang}

Чтобы убедиться, что язык игры определяется автоматически:

1. Очистите [кеш браузера](https://yandex.ru/support/common/ru/browsers-settings/cache){.external}.
1. Откройте игру с [debug-панелью](https://yandex.ru/dev/games/doc/ru/console/debug-panel.md):

   <!-- source: ru/_includes/requirements/start-debug-panel.md -->
   {% list tabs %}

   - Через Консоль разработчика
       1. Откройте [Консоль Яндекс Игр](https://games.yandex.ru/console){.external}.
       1. Выберите нужную игру.
       1. В левом верхнем углу нажмите **Открыть с debug-панелью**.

   - Через адресную строку
       1. Откройте нужную игру.
       1. Добавьте параметр `debug-mode=16` в конец адресной строки браузера.

          Пример ссылки: `https://yandex.ru/games/app/XXXX?debug-mode=16`, где `XXXX` — уникальный идентификатор игры.

   {% endlist %}
   <!-- endsource: ru/_includes/requirements/start-debug-panel.md -->

1. Проверьте, что при первом запуске игры автоопределение языка через SDK работает:

    #|
    || **Фон** | **Текст** | **Значение** ||
    || ![SVG](../../_images/debug-panel/i18n-on.svg) | ##I18N is used## | Автоопределение подключено. ||
    || ![SVG](../../_images/debug-panel/i18n-off.svg) | ##I18N is not used## | Автоопределение не подключено. ||
    |#

    {% note warning %}

    Цвет индикатора 文 должен меняться на зеленый ![SVG](../../_images/debug-panel/i18n-on.svg) на старте, а не в процессе игры. Допустим небольшой интервал для подгрузки языка: на старте до смены цвета индикатора с красного на зеленый может отобразиться загрузочный текст на другом языке.

    {% endnote %}

#### Примеры {#one-lang-examples}

<div class="table-50">

#|
|| **Игра** | **Комментарий** ||
|| ![Игрок нажимает «Играть», и только после этого индикатор языка меняется с зеленого на красный.](../../_images/requirements/2-14-lang-autodetection/i18n-not-ok.gif) | 🚫 Автоопределение подключается после запуска уровня. Такая игра не пройдет модерацию. ||
|| ![Индикатор меняет цвет с красного на зеленый во время загрузки. Игра только на русском, надпись «Загрузка» первоначально на нужном языке.](../../_images/requirements/2-14-lang-autodetection/i18n-ok-1.gif) | ✅ Есть небольшая задержка перед подключением автоопределения, язык изначально правильный (игра только на RU). ||
|| ![Индикатор меняет цвет с красного на зеленый во время загрузки. Вместе с этим меняется надпись «Загружаемся» на «Loading», что соответствует выбранному языку игры — английскому.](../../_images/requirements/2-14-lang-autodetection/i18n-ok-2.gif) | ✅ Есть небольшая задержка перед подключением автоопределения, вместе со сменой цвета индикатора меняется язык текста (язык EN). ||
|#

</div>


### Проверка нескольких языков {#multi-lang}

Модерация проверяет локализацию игры на все языки, указанные на вкладке **Черновик** в поле [Игра переведена на](https://yandex.ru/dev/games/doc/ru/console/add-new-game/draft.md#field-languages).

Повторите для каждого заявленного языка [базовую проверку](#one-lang). Для этого последовательно откройте игру на всех этих языках:

1. На debug-панели нажмите **SDK mocks ⚒️**.
1. В выпадающем списке с обозначением языка (например, **En** ![SVG](../../_images/debug-panel/choose-game-lang-arrow.svg)) выберите нужный. Игра откроется на этом языке в новой вкладке браузера.

{% note info %}

Если в игре есть ручное переключение языка, модерация проверит реализацию только на наличие технических и логических ошибок. Чтобы улучшить опыт игроков, самостоятельно проверьте, что следуйте рекомендациям из [пункта 6.9](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#6-9) Требований к игре.

{% cut "Пример ручного выбора языка" %}

![Меню ручного выбора языка внутри игры.](../../_images/requirements/2-14-lang-autodetection/manual-lang-selection.webp)

{% endcut %}

{% endnote %}

Вердикты модерации:

- ✅ Если все заявленные языки подтянулись — игра пройдет модерацию.
- 🚫 Если хотя бы один язык не переключился (полностью или частично) на выбранный на debug-панели — игра будет отклонена за неперевод. При этом модерация учитывает исключения, которые можно не переводить (см. [калибровочный список](https://yandex.ru/dev/games/doc/ru/requirements/8/2/3.md) к пункту 8.2.3).
- ✅ Допустимо, если после обновления страницы с игрой автоопределение отсутствует в случаях, когда разработчик реализовал сохранение языка в кеше.



## Связаться с модерацией {#forma}

Если вы считаете, что автоопределение языка корректно работает в игре и она была снята с публикации или не допущена до нее по ошибке — заполните форму ниже.

Служба контроля качества модерации перепроверит решение и вернет игру, если она была заблокирована несправедливо.

<div class="cut-button">

{% cut "Открыть форму" %}

<div style="padding: 15px;
         margin: 10px 0;
         background: #FFFFFF;
         border-radius: 10px;
         border: 1px solid var(--yc-color-line-generic);">
      <iframe height="400"
              width="100%"
              frameborder="0"
              id="moderation-2-14-form"
              src="https://forms.yandex.ru/surveys/13487250.5a13659b18018197b1f339b05e2d4dacc05604a6/?answer_choices_52072484=1769421882100&iframe=1">
      </iframe>
</div>

{% endcut %}

</div>

[*lang_code]: Код указан в формате ISO 639-1. Например, `"tr"` означает, что игра сейчас запущена в турецком интерфейсе Яндекс Игр. Все поддерживаемые языки приведены на странице [{#T}](https://yandex.ru/dev/games/doc/ru/concepts/languages-and-domains.md).