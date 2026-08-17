---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-events.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-events.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-events.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-events.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-events.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-events.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-events.md
  - href: ru/sdk/sdk-events.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# События

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

## Пауза и возобновление игры {#pause-resume}

С помощью событий `game_api_pause` и `game_api_resume` платформа сообщает игре, что нужно поставить игровой процесс на паузу или возобновить его. Они упрощают процесс интеграции с нашей платформой и помогают соответствовать требованиям модерации (пункты [1.3](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#1-3) и [4.7](https://yandex.ru/dev/games/doc/ru/concepts/requirements.md#4-7)).

Игры, которые поддерживают события паузы и возобновления игры, могут быть дополнительно дистрибутированы на внешние площадки.


### game_api_pause и game_api_resume {#pause-resume-events}

События помогут вам отследить:

- показ и закрытие полноэкранной или rewarded-рекламы;
- открытие и закрытие окна покупок;
- переключение вкладок браузера;
- сворачивание и разворачивание окна браузера.

Они согласованы с [методами разметки геймплея](https://yandex.ru/dev/games/doc/ru/sdk/sdk-game-events.md#gameplay). При срабатывании `game_api_pause` вызывается метод `GameplayAPI.stop()`, а при срабатывании `game_api_resume` – `GameplayAPI.start()`.

Если игра уже была остановлена с помощью метода `GameplayAPI.stop()` (например, когда игрок открыл меню), и затем срабатывает `game_api_pause`, то при последующем событии `game_api_resume` метод `GameplayAPI.start()` не будет вызван. Это позволяет сохранить текущее состояние игры без нарушения логики разметки геймплея.

Используйте методы `on()` и `off()` из SDK Яндекс Игр для подписки на события и отписки от них соответственно.

#### Пример {#pause-resume-example}

{% list tabs %}

- game_api_pause

    ```javascript showLineNumbers
    const pauseCallback = () => {
        pauseGame(); // Ваша функция, останавливающая игровой цикл и музыку.
        console.log('GAME PAUSED');
    };

    ysdk.on('game_api_pause', pauseCallback); // Подписка на события 'game_api_pause'.
    ysdk.off('game_api_pause', pauseCallback); // Отписка от событий 'game_api_pause'.
    ```

- game_api_resume

    ```javascript showLineNumbers
    const resumeCallback = () => {
        resumeGame(); // Ваша функция, возобновляющая игровой цикл и музыку.
        console.log('GAME RESUMED');
    };

    ysdk.on('game_api_resume', resumeCallback); // Подписка на события 'game_api_resume'.
    ysdk.off('game_api_resume', resumeCallback); // Отписка от событий 'game_api_resume'.
    ```

{% endlist %}

&nbsp; {.empty}


### Полноэкранная реклама на старте игры {#startup-fullscreen-ad}

{% note warning %}

Платформа автоматически показывает полноэкранную рекламу на старте всех игр.

{% endnote %}

В отличие от рекламных блоков, которые игры вызывают через [ysdk.adv.showFullscreenAdv()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-adv.md#full-screen-block), у стартовой рекламы нет прямых callback-функций. Чтобы правильно обрабатывать ее показ, отслеживайте события `game_api_pause` и `game_api_resume`:

1. При получении `game_api_pause` выключите звук из игры и поставьте геймплей на паузу.
2. Дождитесь события `game_api_resume` и возобновите игру.

Это особенно важно для игр, в которых звук и геймплей запускаются сразу.

#### Пример обработки стартовой рекламы {#startup-ad-example}

```javascript showLineNumbers
let gameStarted = false;
let isPaused = false;

// Функция инициализации игры.
function initGame() {
    // Подписываемся на события паузы и возобновления.
    ysdk.on('game_api_pause', handlePause);
    ysdk.on('game_api_resume', handleResume);

    // Проверяем, не находимся ли мы в состоянии паузы.
    // Если да, то ждем resume перед началом игры.
    if (!isPaused) {
        startGame();
    }
}

function handlePause() {
    isPaused = true;
    // Останавливаем звук и геймплей.
    console.log('GAME PAUSED - waiting for resume');
}

function handleResume() {
    isPaused = false;

    // Если игра еще не запущена, запускаем ее.
    if (!gameStarted) {
        startGame();
    } else {
        // Возобновляем звук и геймплей.
    }

    console.log('GAME RESUMED');
}

function startGame() {
    gameStarted = true;

    // Инициализация звука.
    // Запуск игрового цикла.
    console.log('GAME STARTED');
}

// Запускаем инициализацию игры.
initGame();
```



## Прочие события {#other-events}

Вы также можете отслеживать другие события, возникающие при взаимодействии пользователя с приложением.

```typescript showLineNumbers
enum ESdkEventName {
    EXIT = 'EXIT',
    HISTORY_BACK = 'HISTORY_BACK',
    ACCOUNT_SELECTION_DIALOG_OPENED = 'ACCOUNT_SELECTION_DIALOG_OPENED',
    ACCOUNT_SELECTION_DIALOG_CLOSED = 'ACCOUNT_SELECTION_DIALOG_CLOSED',
}

ysdk = {
    EVENTS: {
        EXIT: ESdkEventName.EXIT,
        HISTORY_BACK: ESdkEventName.HISTORY_BACK,
        ACCOUNT_SELECTION_DIALOG_OPENED: ESdkEventName.ACCOUNT_SELECTION_DIALOG_OPENED,
        ACCOUNT_SELECTION_DIALOG_CLOSED: ESdkEventName.ACCOUNT_SELECTION_DIALOG_CLOSED,
    },

    dispatchEvent(eventName: ESdkEventName, detail?: object): Promise<unknown> {},

    on(eventName: ESdkEventName, listener: Function): () => void {}
};
```


### HISTORY_BACK {#history-back}

{% note alert %}

Событие доступно только в случае, если игра запущена на телевизоре.

{% endnote %}

Чтобы отследить нажатие на кнопку **Back**, используйте метод:

```javascript showLineNumbers
ysdk.on(ysdk.EVENTS.HISTORY_BACK, () => {
    // Показ пользователю кастомного диалога игры с возможностью
    // подтвердить выход из игры, перейти к внутренним настройкам, магазину и т. п.
});
```


### EXIT {#exit}

Если пользователь подтвердил выход из игры в кастомном диалоге, открывшемся после нажатия **Back**, то игра должна отправить событие выхода. Для этого используйте метод:

```javascript
ysdk.dispatchEvent(ysdk.EVENTS.EXIT);
```


### Диалог выбора игрового аккаунта {#account-selection-dialog}

Платформа сохраняет игровой прогресс как авторизованных, так и неавторизованных игроков. Пользователь может сначала поиграть без авторизации, а затем войти в аккаунт. В этом случае у него будет два разных прогресса: один под логином, другой — без него. Платформа покажет диалоговое окно, в котором пользователь сможет сравнить сохранения по длительности игры, дате последнего входа и другим параметрам и выбрать, какой прогресс использовать.

Если вы часто синхронизируете данные игрока или храните игровой прогресс на своем сервере, отслеживайте смену прогресса после выбора аккаунта в диалоговом окне.

В SDK для этого есть два события:

- `ACCOUNT_SELECTION_DIALOG_OPENED` — открытие диалога.
- `ACCOUNT_SELECTION_DIALOG_CLOSED` — закрытие диалога.

При открытии диалога можно приостановить регулярную синхронизацию данных игрока. При закрытии — выйти в главное меню или перезапустить игру и заново запросить объект игрока.

#### Пример {#account-selection-dialog-example}

```javascript showLineNumbers
// Подписка на событие открытия диалога выбора аккаунта.
ysdk.on(ysdk.EVENTS.ACCOUNT_SELECTION_DIALOG_OPENED, () => {
    // Приостанавливаем синхронизацию данных игрока.
});

// Подписка на событие закрытия диалога выбора аккаунта.
ysdk.on(ysdk.EVENTS.ACCOUNT_SELECTION_DIALOG_CLOSED, async () => {
    // Выходим в главное меню игры или перезагружаем страницу.
    // ...

    // Перезапрашиваем данные игрока.
    const player = await ysdk.getPlayer();
    const data = await player.getData();
});
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
