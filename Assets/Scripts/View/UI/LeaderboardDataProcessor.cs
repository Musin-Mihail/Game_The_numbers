using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YandexGames;

namespace View.UI
{
    /// <summary>
    /// Фильтрует таблицу лидеров: топ-3, соседи текущего игрока, минимум 10 строк.
    /// </summary>
    public class LeaderboardDataProcessor
    {
        public const int MinEntriesToShow = 10;
        public const int PlayerNeighborsCount = 3;

        public List<LeaderboardPlayer> ProcessLeaderboardData(LeaderboardTable table)
        {
            if (table?.players == null || table.players.Length == 0)
            {
                return new List<LeaderboardPlayer>();
            }

            var allPlayers = table.players.ToList();
            var currentPlayerRank = table.currentPlayerRank;
            var addedPlayerIDs = new HashSet<string>();
            var playersToShow = new List<LeaderboardPlayer>();

            var topPlayers = allPlayers.Where(p => p.rank <= 3).OrderBy(p => p.rank);
            playersToShow.AddRange(topPlayers.Where(player => addedPlayerIDs.Add(player.uniqueID)));

            var neighbors = allPlayers
                .Where(p => Mathf.Abs(p.rank - currentPlayerRank) <= PlayerNeighborsCount)
                .OrderBy(p => p.rank);
            playersToShow.AddRange(neighbors.Where(player => addedPlayerIDs.Add(player.uniqueID)));

            if (playersToShow.Count < MinEntriesToShow && allPlayers.Count > playersToShow.Count)
            {
                var maxRankInList = playersToShow.Count > 0 ? playersToShow.Max(p => p.rank) : 0;
                var potentialAdditions = allPlayers
                    .Where(p => p.rank > maxRankInList)
                    .OrderBy(p => p.rank);

                foreach (var player in potentialAdditions)
                {
                    if (playersToShow.Count >= MinEntriesToShow) break;
                    if (addedPlayerIDs.Add(player.uniqueID))
                    {
                        playersToShow.Add(player);
                    }
                }
            }

            return playersToShow.OrderBy(p => p.rank).ToList();
        }
    }
}
