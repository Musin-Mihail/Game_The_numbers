using UnityEngine;

namespace YandexGames
{
    /// <summary>
    /// Кодек облака PluginYG2: <c>player.setData({ saves: [jsonString] })</c>.
    /// Чтение поддерживает и сырой JSON, и <c>JSON.stringify(data.saves)</c> как в Storage_yg.
    /// </summary>
    public static class CloudSaveCodec
    {
        public const string NoDataSentinel = "no data";

        public static string Encode(GameSaveData data)
        {
            return JsonUtility.ToJson(data ?? new GameSaveData());
        }

        /// <summary>
        /// Как <c>JSON.stringify([jsonString])</c> в браузере — то, что PluginYG2 слал в C#.
        /// </summary>
        public static string WrapAsPluginYg2Callback(string saveJson)
        {
            saveJson ??= "";
            return "[\"" + EscapeJsonString(saveJson) + "\"]";
        }

        public static bool TryDecode(string payload, out GameSaveData data)
        {
            data = new GameSaveData();
            if (IsMissing(payload))
            {
                return false;
            }

            var json = Unwrap(payload);
            if (string.IsNullOrEmpty(json) || json[0] != '{')
            {
                return false;
            }

            try
            {
                var parsed = JsonUtility.FromJson<GameSaveData>(json);
                if (parsed == null)
                {
                    return false;
                }

                data = parsed;
                data.gridCells ??= new();
                data.seenUpdateVersions ??= new();
                data.seenMigrationIds ??= new();
                data.statistics ??= new();
                data.actionCounters ??= new();
                data.gridState ??= "";
                return true;
            }
            catch
            {
                data = new GameSaveData();
                return false;
            }
        }

        public static string Unwrap(string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                return payload;
            }

            var trimmed = payload.Trim();
            if (trimmed.Length >= 4 && trimmed.StartsWith("[\"") && trimmed.EndsWith("\"]"))
            {
                var data = trimmed.Remove(0, 2);
                data = data.Remove(data.Length - 2, 2);
                data = data.Replace(@"\\\", "\u0002");
                data = data.Replace(@"\", "");
                data = data.Replace("\u0002", @"\");
                return data;
            }

            return trimmed;
        }

        public static bool IsMissing(string payload)
        {
            return string.IsNullOrEmpty(payload) || payload == NoDataSentinel;
        }

        private static string EscapeJsonString(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
