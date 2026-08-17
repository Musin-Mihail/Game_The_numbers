using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YandexGames
{
    /// <summary>
    /// Тонкий мост Unity WebGL → window.ysdk. Имя объекта фиксировано для SendMessage.
    /// </summary>
    public class YandexGamesSdk : MonoBehaviour
    {
        public const string GameObjectName = "YandexGamesSdk";

        public static YandexGamesSdk Instance { get; private set; }
        public static bool IsReady { get; private set; }
        public static string Lang { get; private set; } = "ru";
        public static bool IsAuthorized { get; private set; }
        public static string PlayerId { get; private set; } = "";
        public static GameSaveData Saves { get; private set; } = new();
        public static ProductInfo[] Catalog { get; private set; } = Array.Empty<ProductInfo>();

        public static event Action Ready;
        public static event Action DefaultSaves;
        public static event Action<LeaderboardTable> LeaderboardReceived;
        public static event Action<string> PurchaseSuccess;
        public static event Action<string> PurchaseFailed;
        public static event Action<string> Rewarded;
        public static event Action<bool> InterstitialClosed;
        public static event Action<bool> PauseChanged;

        private static bool _loadingReadySent;
        private static bool _gameplayActive;
        private static bool _paused;
        private static string _editorSavePath;
        private static bool _notifiedDefaultSaves;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (Instance != null)
            {
                return;
            }

            var go = new GameObject(GameObjectName);
            DontDestroyOnLoad(go);
            go.AddComponent<YandexGamesSdk>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.name = GameObjectName;

#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_Bind(GameObjectName);
#else
            InitEditorStub();
#endif
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public static void SaveProgress()
        {
            if (!IsReady)
            {
                Debug.LogWarning("[YandexGamesSdk] SaveProgress до инициализации SDK.");
                return;
            }

            Saves.idSave++;
            var json = CloudSaveCodec.Encode(Saves);
#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_SaveCloud(json, 1);
#else
            WriteEditorSave(json);
#endif
        }

        public static void SetLeaderboardScore(string name, int score)
        {
            if (!IsAuthorized)
            {
                return;
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_SetLeaderboard(name, score);
#endif
        }

        public static void RequestLeaderboard(string name, int quantityTop, int quantityAround)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_GetLeaderboard(name, quantityTop, quantityAround, IsAuthorized ? 1 : 0);
#else
            LeaderboardReceived?.Invoke(new LeaderboardTable
            {
                technoName = name,
                currentPlayerRank = 0,
                players = Array.Empty<LeaderboardPlayer>()
            });
#endif
        }

        public static void ShowInterstitial()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SetPausedInternal(true);
            YandexGames_ShowInterstitial();
#else
            InterstitialClosed?.Invoke(true);
#endif
        }

        public static void ShowRewarded(string rewardId)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SetPausedInternal(true);
            YandexGames_ShowRewarded(rewardId);
#else
            Rewarded?.Invoke(rewardId);
#endif
        }

        public static void Purchase(string productId)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SetPausedInternal(true);
            YandexGames_Purchase(productId);
#else
            var product = GetProduct(productId);
            if (product == null)
            {
                PurchaseFailed?.Invoke(productId);
                return;
            }

            PurchaseSuccess?.Invoke(productId);
#endif
        }

        public static void ConsumePurchase(string productId)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_ConsumePurchase(productId);
#endif
        }

        public static ProductInfo GetProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId) || Catalog == null)
            {
                return null;
            }

            for (var i = 0; i < Catalog.Length; i++)
            {
                if (Catalog[i] != null && Catalog[i].id == productId)
                {
                    return Catalog[i];
                }
            }

            return null;
        }

        public static void NotifyGameReady()
        {
            if (_loadingReadySent)
            {
                return;
            }

            _loadingReadySent = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_LoadingReady();
#endif
        }

        public static void NotifyGameplayStart()
        {
            if (_gameplayActive)
            {
                return;
            }

            _gameplayActive = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_GameplayStart();
#endif
        }

        public static void NotifyGameplayStop()
        {
            if (!_gameplayActive)
            {
                return;
            }

            _gameplayActive = false;
#if UNITY_WEBGL && !UNITY_EDITOR
            YandexGames_GameplayStop();
#endif
        }

#if UNITY_EDITOR
        public static void InitEditorForTests(string savePath, bool wipe = true)
        {
            IsReady = false;
            _editorSavePath = savePath;
            _notifiedDefaultSaves = false;
            _loadingReadySent = false;
            _gameplayActive = false;
            _paused = false;
            IsAuthorized = false;
            PlayerId = "";
            Lang = "ru";
            Catalog = new[]
            {
                new ProductInfo
                {
                    id = "disable_counters",
                    title = "Disable counters",
                    price = "10 YAN",
                    priceValue = "10",
                    priceCurrencyCode = "YAN",
                    consumed = true
                }
            };

            if (wipe && !string.IsNullOrEmpty(savePath) && File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            ApplyCloudPayload(wipe ? "" : ReadEditorSave());
            MarkReady();
        }
#endif

        public void OnJsEnvironment(string json)
        {
            var env = JsonUtility.FromJson<EnvironmentDto>(json) ?? new EnvironmentDto();
            Lang = string.IsNullOrEmpty(env.lang) ? "ru" : env.lang;
            PlayerId = env.playerId ?? "";
            IsAuthorized = env.authorized;
        }

        public void OnJsCatalog(string json)
        {
            var parsed = JsonUtility.FromJson<ProductCatalogJson>(json);
            Catalog = parsed?.items ?? Array.Empty<ProductInfo>();
        }

        public void OnJsCloudSaves(string payload)
        {
            ApplyCloudPayload(payload);
            MarkReady();
        }

        public void OnJsLeaderboard(string json)
        {
            var table = JsonUtility.FromJson<LeaderboardTable>(json);
            if (table != null)
            {
                table.players ??= Array.Empty<LeaderboardPlayer>();
                LeaderboardReceived?.Invoke(table);
            }
        }

        public void OnJsPurchaseSuccess(string productId)
        {
            SetPausedInternal(false);
            PurchaseSuccess?.Invoke(productId);
        }

        public void OnJsPurchaseFailed(string productId)
        {
            SetPausedInternal(false);
            PurchaseFailed?.Invoke(productId);
        }

        public void OnJsRewarded(string rewardId)
        {
            Rewarded?.Invoke(rewardId);
        }

        public void OnJsRewardedClosed()
        {
            SetPausedInternal(false);
        }

        public void OnJsInterstitialClosed(string wasShown)
        {
            SetPausedInternal(false);
            InterstitialClosed?.Invoke(wasShown == "true" || wasShown == "1");
        }

        public void OnPlatformPause()
        {
            SetPausedInternal(true);
        }

        public void OnPlatformResume()
        {
            SetPausedInternal(false);
        }

        private static void ApplyCloudPayload(string payload)
        {
            if (!CloudSaveCodec.TryDecode(payload, out var data))
            {
                Saves = new GameSaveData();
                if (!_notifiedDefaultSaves)
                {
                    _notifiedDefaultSaves = true;
                    DefaultSaves?.Invoke();
                }

                return;
            }

            Saves = data;
        }

        private static void MarkReady()
        {
            if (IsReady)
            {
                return;
            }

            IsReady = true;
            Ready?.Invoke();
        }

        private static void SetPausedInternal(bool paused)
        {
            if (_paused == paused)
            {
                return;
            }

            _paused = paused;
            AudioListener.pause = paused;
            if (paused)
            {
                NotifyGameplayStop();
            }
            else
            {
                NotifyGameplayStart();
            }

            PauseChanged?.Invoke(paused);
        }

        private static void InitEditorStub()
        {
            Lang = MapSystemLanguage();
            IsAuthorized = false;
            PlayerId = "";
            Catalog = new[]
            {
                new ProductInfo
                {
                    id = "disable_counters",
                    title = "Disable counters",
                    price = "10 YAN",
                    priceValue = "10",
                    priceCurrencyCode = "YAN",
                    consumed = true
                }
            };

            var json = ReadEditorSave();
            ApplyCloudPayload(json);
            MarkReady();
        }

        private static string MapSystemLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional: return "zh";
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Turkish: return "tr";
                default: return "en";
            }
        }

        private static string EditorSavePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_editorSavePath))
                {
                    return _editorSavePath;
                }

                return Path.Combine(Application.persistentDataPath, "yandex-games-save.json");
            }
        }

        private static string ReadEditorSave()
        {
            try
            {
                if (File.Exists(EditorSavePath))
                {
                    return File.ReadAllText(EditorSavePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[YandexGamesSdk] Не удалось прочитать editor-save: " + e.Message);
            }

            return "";
        }

        private static void WriteEditorSave(string json)
        {
            try
            {
                File.WriteAllText(EditorSavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[YandexGamesSdk] Не удалось записать editor-save: " + e.Message);
            }
        }

        [Serializable]
        private class EnvironmentDto
        {
            public string lang;
            public string playerId;
            public bool authorized;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void YandexGames_Bind(string gameObjectName);
        [DllImport("__Internal")] private static extern void YandexGames_SaveCloud(string json, int flush);
        [DllImport("__Internal")] private static extern void YandexGames_SetLeaderboard(string name, int score);
        [DllImport("__Internal")] private static extern void YandexGames_GetLeaderboard(string name, int quantityTop, int quantityAround, int includeUser);
        [DllImport("__Internal")] private static extern void YandexGames_ShowInterstitial();
        [DllImport("__Internal")] private static extern void YandexGames_ShowRewarded(string rewardId);
        [DllImport("__Internal")] private static extern void YandexGames_Purchase(string productId);
        [DllImport("__Internal")] private static extern void YandexGames_ConsumePurchase(string productId);
        [DllImport("__Internal")] private static extern void YandexGames_LoadingReady();
        [DllImport("__Internal")] private static extern void YandexGames_GameplayStart();
        [DllImport("__Internal")] private static extern void YandexGames_GameplayStop();
#endif
    }
}
