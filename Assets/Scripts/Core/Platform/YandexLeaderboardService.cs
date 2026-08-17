using System;
using Interfaces;
using UnityEngine;
using YandexGames;

namespace Core.Platform
{
    /// <summary>
    /// Реализация сервиса таблицы лидеров для Yandex Games.
    /// </summary>
    public class YandexLeaderboardService : ILeaderboardService, IDisposable
    {
        private readonly string _leaderboardName;

        public event Action<LeaderboardTable> OnEntriesReceived;

        public YandexLeaderboardService(string leaderboardName)
        {
            _leaderboardName = leaderboardName;
            YandexGamesSdk.LeaderboardReceived += OnSdkLeaderboardReceived;
        }

        public void Dispose()
        {
            YandexGamesSdk.LeaderboardReceived -= OnSdkLeaderboardReceived;
        }

        public void UpdateLeaderboard(int score)
        {
            if (YandexGamesSdk.IsAuthorized)
            {
                YandexGamesSdk.SetLeaderboardScore(_leaderboardName, score);
                Debug.Log($"Таблица лидеров '{_leaderboardName}' обновлена с результатом: {score}");
            }
            else
            {
                Debug.LogWarning("Игрок не авторизован. Результат не отправлен в таблицу лидеров.");
            }
        }

        public void RequestEntries(int quantityTop, int quantityAround)
        {
            if (!YandexGamesSdk.IsAuthorized)
            {
                Debug.LogWarning("Player is not authorized. Cannot fetch leaderboard.");
                return;
            }

            YandexGamesSdk.RequestLeaderboard(_leaderboardName, quantityTop, quantityAround);
        }

        private void OnSdkLeaderboardReceived(LeaderboardTable table)
        {
            if (table == null || table.technoName != _leaderboardName)
            {
                return;
            }

            OnEntriesReceived?.Invoke(table);
        }
    }
}
