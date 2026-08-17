using Core;
using Core.Events;
using Interfaces;
using UnityEngine;
using YandexGames;

namespace View.UI
{
    /// <summary>
    /// Управляет видимостью окна статистики и обновлением таблицы лидеров.
    /// </summary>
    public class StatisticsWindowManager : MonoBehaviour
    {
        private GameObject _statisticsWindow;
        private LeaderboardView _leaderboardView;
        private ILeaderboardService _leaderboardService;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            _statisticsWindow = transform.Find("Statistics")?.gameObject;
            _leaderboardView = GetComponent<LeaderboardView>() ?? UnityEngine.Object.FindAnyObjectByType<LeaderboardView>(FindObjectsInactive.Include);

            if (_statisticsWindow != null)
            {
                var btnHideStatistics = _statisticsWindow.transform.FindComponentInChildren<UnityEngine.UI.Button>("Closed");
                btnHideStatistics?.onClick.AddListener(() => GlobalEvents.OnHideStatistics?.Invoke());
            }
        }

        private void OnEnable()
        {
            GlobalEvents.OnShowStatistics += ShowStatisticsWindow;
            GlobalEvents.OnHideStatistics += HideStatisticsWindow;
            BindLeaderboardService();
        }

        private void OnDisable()
        {
            GlobalEvents.OnShowStatistics -= ShowStatisticsWindow;
            GlobalEvents.OnHideStatistics -= HideStatisticsWindow;
            if (_leaderboardService != null)
            {
                _leaderboardService.OnEntriesReceived -= OnLeaderboardReceived;
            }
        }

        private void Start()
        {
            if (_statisticsWindow)
            {
                _statisticsWindow.SetActive(false);
            }
        }

        private void ShowStatisticsWindow()
        {
            if (!_statisticsWindow) return;
            _statisticsWindow.SetActive(true);
            BindLeaderboardService();
            _leaderboardService?.RequestEntries(10, 3);
        }

        private void OnLeaderboardReceived(LeaderboardTable table)
        {
            if (table == null || table.technoName != GameConstants.LeaderboardName) return;

            if (_leaderboardView)
            {
                _leaderboardView.BuildLeaderboard(table);
            }
            else
            {
                Debug.LogError("LeaderboardController не найден в StatisticsWindowManager!", this);
            }
        }

        private void HideStatisticsWindow()
        {
            if (_statisticsWindow)
            {
                _statisticsWindow.SetActive(false);
            }
        }

        private void BindLeaderboardService()
        {
            if (_leaderboardService != null) return;
            _leaderboardService = ServiceProvider.GetService<ILeaderboardService>();
            if (_leaderboardService != null)
            {
                _leaderboardService.OnEntriesReceived += OnLeaderboardReceived;
            }
        }
    }
}
