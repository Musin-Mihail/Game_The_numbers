---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-shortcut.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-shortcut.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-shortcut.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-shortcut.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-shortcut.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-shortcut.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-shortcut.md
  - href: ru/sdk/sdk-shortcut.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Ярлык на рабочий стол

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

С помощью нативного диалогового окна вы можете предложить пользователю добавить на рабочий стол ярлык — ссылку на игру.


Прежде чем вывести диалоговое окно, убедитесь, что опция доступна.


## Проверка возможности добавления ярлыка {#can-add-shortcut}

Доступность опции зависит от устройства, внутренних правил браузера и ограничений платформы [Яндекс Игры](https://yandex.ru/games/){.external}.

Чтобы убедиться, что ярлык можно добавить, используйте метод `ysdk.shortcut.canShowPrompt()`:

```javascript showLineNumbers
const ysdk = await YaGames.init();

const prompt = await ysdk.shortcut.canShowPrompt();

if (prompt.canShow) {
  // Здесь можно показать кнопку для добавления ярлыка.
}
```

## Вызов диалогового окна {#dialog-add-shortcut}

После проверки можно показать в игре кнопку (или другой элемент интерфейса), при нажатии на которую откроется диалоговое окно для добавления ярлыка.

Чтобы вызвать окно, используйте метод `ysdk.shortcut.showPrompt()`:

```javascript showLineNumbers
const ysdk = await YaGames.init();

const result = await ysdk.shortcut.showPrompt();

if (result.outcome === 'accepted') {
  // Здесь можно начислить награду за добавление ярлыка.
}
```

При первом вызове метода создается ярлык для каталога [Яндекс Игр](https://yandex.ru/games/){.external}. Если он уже есть, то будет создан ярлык со ссылкой на саму игру.


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
