using Core.Events;
using Interfaces;
using UnityEngine;
using YG;

namespace Core
{
    /// <summary>
    /// Отвечает за обновление счета игрока в таблице лидеров при изменении статистики,
    /// а также за обновление рекорда.
    /// </summary>
    public class LeaderboardUpdater : MonoBehaviour
    {
        private ILeaderboardService _leaderboardService;

        /// <summary>
        /// Инициализация зависимостей, полученных из GameBootstrap.
        /// </summary>
        public void Initialize(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Подписывается на события.
        /// </summary>
        private void OnEnable()
        {
            GlobalEvents.OnStatisticsChanged += OnStatisticsChanged;
        }

        /// <summary>
        /// Отписывается от событий.
        /// </summary>
        private void OnDisable()
        {
            GlobalEvents.OnStatisticsChanged -= OnStatisticsChanged;
        }

        /// <summary>
        /// Обрабатывает изменение статистики, обновляет таблицу лидеров и рекорд.
        /// </summary>
        /// <param name="statsData">Данные статистики (счет, множитель).</param>
        private void OnStatisticsChanged((long score, int multiplier) statsData)
        {
            if (statsData.score <= YG2.saves.record) return;
            YG2.saves.record = statsData.score;
            _leaderboardService?.UpdateLeaderboard((int)statsData.score);
            Debug.Log($"Новый рекорд установлен: {YG2.saves.record}");
            GlobalEvents.OnStatisticsChanged?.Invoke(statsData);
        }
    }
}