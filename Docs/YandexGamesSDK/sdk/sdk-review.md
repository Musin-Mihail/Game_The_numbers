---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-review.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-review.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-review.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-review.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-review.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-review.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-review.md
  - href: ru/sdk/sdk-review.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Оценка игры

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

Вы можете попросить пользователя оценить игру и написать комментарий во всплывающем окне. Оно появится в момент запроса оценки.

Прежде чем запросить оценку игры, убедитесь, что опция доступна для этого пользователя: нужно, чтобы он был авторизован и не оценивал игру ранее.

Подробнее о работе с оценками см. на странице [Отзывы](https://yandex.ru/dev/games/doc/ru/console/reviews.md).

## Проверка возможности запросить оценку {#can-review}

Чтобы узнать, можно ли запросить оценку игры, используйте метод `ysdk.feedback.canReview()`.

Он возвращает `Promise<Object>`, который переходит в состояние `resolved`. Возвращаемый объект содержит ключ `value` со значением `true/false`. По нему можно узнать, есть ли возможность запросить оценку:


#|
|| **Значение** | **Описание** ||
|| `value: true` | Запросить можно. ||
||::{align="center"} `value: false` | Запросить нельзя.

Причина отказа указывается в виде строкового значения в ключе `reason`:
- `NO_AUTH` — пользователь не авторизован.
- `GAME_RATED` — пользователь уже оценивал игру.
- `REVIEW_ALREADY_REQUESTED` — запрос уже отправлен, ожидаются действия пользователя.
- `REVIEW_WAS_REQUESTED` — запрос уже отправлен, пользователь совершил действие: поставил оценку или закрыл всплывающее окно.
- `UNKNOWN` — запрос не был отправлен, ошибка на стороне Яндекса. ||
|#


## Запрос оценки {#request-review}


{% note alert %}

Запросить оценку игры можно только один раз за сессию. Обязательно используйте метод [`ysdk.feedback.canReview()`](#can-review) перед выполнением запроса.

{% endnote %}


Чтобы предложить пользователю оценить игру и написать комментарий, используйте метод `ysdk.feedback.requestReview()`.

Он возвращает `Promise<Object>`, который переходит в состояние `resolved`. Возвращаемый объект содержит ключ `feedbackSent` со значением `true/false`. По нему можно узнать действие пользователя:

#|
|| **Значение**| **Описание** ||
|| `feedbackSent: true` | Пользователь оценил игру. ||
|| `feedbackSent: false` | Пользователь закрыл всплывающее окно. ||
|#


Если перед выполнением запроса вы не использовали метод `ysdk.feedback.canReview()`, значение `feedbackSent: false` может сопровождаться ошибкой `use canReview before requestReview`.

#### Пример {#request-review-example}

```javascript showLineNumbers
const ysdk = await YaGames.init();

const { value, reason } = await ysdk.feedback.canReview();

if (value) {
    const { sentFeedback } = ysdk.feedback.requestReview();
} else {
    console.log(reason);
}
```


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
