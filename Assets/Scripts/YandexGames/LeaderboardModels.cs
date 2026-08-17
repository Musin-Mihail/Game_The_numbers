using System;

namespace YandexGames
{
    [Serializable]
    public class LeaderboardPlayer
    {
        public int rank;
        public string name;
        public int score;
        public string photo;
        public string uniqueID;
    }

    [Serializable]
    public class LeaderboardTable
    {
        public string technoName;
        public int currentPlayerRank;
        public LeaderboardPlayer[] players;
    }
}
