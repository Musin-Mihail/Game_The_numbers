---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-game-events.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-game-events.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-game-events.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-game-events.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-game-events.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-game-events.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-game-events.md
  - href: ru/sdk/sdk-game-events.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Загрузка игры и разметка геймплея

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

## Загрузка игры {#gameready}

Яндекс Игры работают по всему миру, поэтому сбор статистики важен для улучшения скорости загрузки и доступности игр. Например, на основе этих данных мы решаем, где развернуть новые дата-центры.

Понимание, когда игры загружаются, позволит нам добавить новые функциональные возможности: показ общего для всех игр загрузочного экрана, скриншотов игры или отзывов. Это повысит привлекательность игр.

### ysdk.features.LoadingAPI.ready() {#ready}

Метод нужно вызывать, когда игра загрузила все ресурсы и готова к взаимодействию с пользователем.

Убедитесь, что в момент вызова метода из Game Ready в игре:

- все элементы готовы к взаимодействию с игроком;
- нет экранов загрузки.

Отследить метрику Game Ready можно во вкладке [Performance в DevTools](https://yandex.ru/dev/games/doc/ru/concepts/performance.md).

### Пример {#ready-example}

{% list tabs %}

- Вариант с await

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  // Сообщаем платформе, что игра загрузилась и можно начинать играть.
  ysdk.features.LoadingAPI?.ready()
  ```

- Вариант без await

  ```javascript showLineNumbers
  YaGames.init()
      .then((ysdk) => {
          // Сообщаем платформе, что игра загрузилась и можно начинать играть.
          ysdk.features.LoadingAPI?.ready()
      })
      .catch(console.error);
  ```

{% endlist %}


## Геймплей {#gameplay}

Нам важно отслеживать, когда и как пользователи взаимодействуют с играми. Для этого в SDK есть специальные методы, которые позволяют размечать начало и остановку игрового процесса. Их использование поможет нам повысить точность рекомендаций в каталоге, распространить игры на большее число площадок и сформировать дополнительные метрики в Консоли разработчика.

### ysdk.features.GameplayAPI.start() {#start}

Метод нужно вызывать, когда игрок начинает или возобновляет игровой процесс. К таким случаям относятся:

- запуск уровня;
- закрытие меню;
- снятие с паузы;
- возобновление игры после показа рекламы;
- возвращение в текущую вкладку браузера.

Убедитесь, что после отправки события `GameplayAPI.start()` игровой процесс сразу запущен.

### ysdk.features.GameplayAPI.stop() {#stop}

Метод нужно вызывать, когда игрок приостанавливает или завершает игровой процесс. К таким случаям относятся:

- прохождение уровня или проигрыш;
- вызов меню;
- пауза в игре;
- показ полноэкранной или rewarded-рекламы;
- уход в другую вкладку браузера.

Убедитесь, что после отправки события `GameplayAPI.stop()` игровой процесс остановлен.

{% note warning %}

В момент возобновления игрового процесса снова вызовите метод `ysdk.features.GameplayAPI.start()`.

{% endnote %}

### Пример {#start-stop-example}

{% list tabs %}

- Вариант с await

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  // Сообщаем о старте геймплея.
  ysdk.features.GameplayAPI?.start()

  // Игровой процесс активен.

  // Сообщаем об остановке геймплея:
  // игрок вышел в меню, прошел уровень или планируется показ рекламы.
  ysdk.features.GameplayAPI?.stop()
  ```

- Вариант без await

  ```javascript showLineNumbers
  YaGames.init()
      .then((ysdk) => {
          // Сообщаем о старте геймплея.
          ysdk.features.GameplayAPI?.start()

          // Игровой процесс активен.

          // Сообщаем об остановке геймплея:
          // игрок вышел в меню, прошел уровень или планируется показ рекламы.
          ysdk.features.GameplayAPI?.stop()
      });
  ```

{% endlist %}


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
