using System;
using System.Collections.Generic;
using Core;

namespace YandexGames
{
    /// <summary>
    /// JSON прогресса. Имена полей совпадают с SavesYG PluginYG2 — иначе облако текущих игроков пустое.
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public int idSave;
        public bool isTutorialCompleted;
        public bool isTopLineVisible = true;
        public long record;
        public List<CellDataSerializable> gridCells = new();
        public string gridState = "";
        public StatisticsModelSerializable statistics = new();
        public ActionCountersModelSerializable actionCounters = new();
        public List<int> seenUpdateVersions = new();
        public List<string> seenMigrationIds = new();
    }
}
