---
metadata:
  - name: generator
    content: Diplodoc Platform v5.52.0
alternate:
  - https://yandex.ru/dev/games/doc/en/sdk/sdk-server-time.md
  - https://yandex.ru/dev/games/doc/hi/sdk/sdk-server-time.md
  - https://yandex.ru/dev/games/doc/ko/sdk/sdk-server-time.md
  - https://yandex.ru/dev/games/doc/ru/sdk/sdk-server-time.md
  - https://yandex.ru/dev/games/doc/tr/sdk/sdk-server-time.md
  - https://yandex.ru/dev/games/doc/vi/sdk/sdk-server-time.md
  - https://yandex.ru/dev/games/doc/zh/sdk/sdk-server-time.md
  - href: ru/sdk/sdk-server-time.md
    type: text/markdown
    title: Markdown version
  - href: ../llms.txt
    type: text/markdown
    title: llms.txt
---
> **Documentation Index:** Fetch the complete configuration index at https://yandex.ru/dev/games/doc/ru/llms.txt

# Серверное время

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

SDK Яндекс Игр позволяет получить время, синхронизированное с сервером. Метод полезен для:

- **Защиты от накруток**: пользователи не смогут влиять на игровые процессы, изменяя время на своем устройстве.
- **Игровых событий**: на его базе вы можете добавлять активности и награды, для которых важен доверенный источник времени (например, ежедневные или еженедельные бонусы, сезонные события и квесты).

## ysdk.serverTime() {#server-time}

Метод возвращает `timestamp`, серверное время в миллисекундах, одинаковое на всех устройствах. Он похож на `Date.now()` форматом результата, однако последний возвращает время устройства пользователя, которое может отличаться от серверного и не защищено от накруток со стороны игроков. Метод `ysdk.serverTime()` устойчив к накрутке системного времени на устройстве, что делает его более надежным.

Вызывайте его каждый раз, когда вам требуется получить актуальное время.

{% list tabs %}

- Вариант с await

  ```javascript showLineNumbers
  const ysdk = await YaGames.init();

  // Возвращает время в мс, синхронизированное с сервером.
  ysdk.serverTime(); // Например, 1720613073778.

  // Спустя время вызываем еще раз.
  ysdk.serverTime(); // Например, 1720613132635.
  ```

- Вариант без await

  ```javascript showLineNumbers
  YaGames.init().then(ysdk => {

      // Возвращает время в мс, синхронизированное с сервером.
      ysdk.serverTime(); // Например, 1720613073778.

      // Спустя время вызываем еще раз.
      ysdk.serverTime(); // Например, 1720613132635.
  });
  ```

{% endlist %}

## Примеры реализации ежедневных наград {#reward-examples}

- Используется `ysdk.serverTime()` для получения надежного серверного времени.
- Данные сохраняются через [player.setData()](https://yandex.ru/dev/games/doc/ru/sdk/sdk-player.md#ingame-data).
- Реализована защита от повторного получения награды.
- Время сравнивается безопасным способом.

{% note warning %}

Функция `giveReward()` в примерах — это ваша реализация начисления награды игроку.

{% endnote %}

### Награда спустя 24 часа после последнего посещения игры {#twenty-four-hours-reward}

```javascript showLineNumbers
YaGames.init().then(async ysdk => {
    // Инициализация игрока.
    const player = await ysdk.getPlayer();

    // Получаем сохраненные данные.
    const data = await player.getData();

    // Текущее серверное время.
    const currentTime = ysdk.serverTime();

    // Время последнего получения награды (если нет, используем 0).
    const lastRewardTime = data.lastRewardTime || 0;

    // 24 часа в миллисекундах.
    const DAY_IN_MS = 24 * 60 * 60 * 1000;

    if (currentTime - lastRewardTime >= DAY_IN_MS) {
        // Прошло более 24 часов — можно выдать награду.
        await giveReward(); // Ваша функция начисления награды.

        // Сохраняем новое время получения награды.
        await player.setData({
            lastRewardTime: currentTime
        });
    }
});
```

### Награда один раз в календарные сутки (сброс в полночь по UTC) {#calendar-day-reward}

```javascript showLineNumbers
YaGames.init().then(async ysdk => {
    // Инициализация игрока.
    const player = await ysdk.getPlayer();

    // Получаем сохраненные данные.
    const data = await player.getData();

    // Текущее серверное время.
    const currentTime = ysdk.serverTime();

    // Получаем дату последней награды в формате "YYYY-MM-DD".
    const lastRewardDate = data.lastRewardDate || '';

    // Получаем текущую дату в формате "YYYY-MM-DD".
    const currentDate = new Date(currentTime).toISOString().split('T')[0];

    if (currentDate !== lastRewardDate) {
        // Сегодня награда еще не была получена.
        await giveReward(); // Ваша функция начисления награды.

        // Сохраняем дату получения награды.
        await player.setData({
            lastRewardDate: currentDate
        });
    }
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
