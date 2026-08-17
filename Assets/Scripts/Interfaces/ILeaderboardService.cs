using System;
using YandexGames;

namespace Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса таблицы лидеров.
    /// </summary>
    public interface ILeaderboardService
    {
        event Action<LeaderboardTable> OnEntriesReceived;

        void UpdateLeaderboard(int score);

        void RequestEntries(int quantityTop, int quantityAround);
    }
}
