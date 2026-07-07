using Core;
using Core.Events;
using UnityEngine;
using YG;
using YG.Utils.LB;

namespace View.UI
{
    /// <summary>
    /// Управляет видимостью окна статистики и обновлением таблицы лидеров.
    /// </summary>
    public class StatisticsWindowManager : MonoBehaviour
    {
        private GameObject _statisticsWindow;
        private LeaderboardView _leaderboardView;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            _statisticsWindow = transform.Find("Statistics")?.gameObject;
            _leaderboardView = GetComponent<LeaderboardView>() ?? UnityEngine.Object.FindFirstObjectByType<LeaderboardView>(FindObjectsInactive.Include);
            
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

            YG2.onGetLeaderboard += OnLeaderboardReceived;
        }

        private void OnDisable()
        {
            GlobalEvents.OnShowStatistics -= ShowStatisticsWindow;
            GlobalEvents.OnHideStatistics -= HideStatisticsWindow;

            YG2.onGetLeaderboard -= OnLeaderboardReceived;
        }

        private void Start()
        {
            if (_statisticsWindow)
            {
                _statisticsWindow.SetActive(false);
            }
        }

        /// <summary>
        /// Показывает окно статистики и запрашивает обновление таблицы лидеров.
        /// </summary>
        private void ShowStatisticsWindow()
        {
            if (!_statisticsWindow) return;
            _statisticsWindow.SetActive(true);

            if (YG2.player.auth)
            {
                YG2.GetLeaderboard(GameConstants.LeaderboardName, 10, 3);
            }
            else
            {
                Debug.LogWarning("Player is not authorized. Cannot fetch leaderboard.");
            }
        }

        /// <summary>
        /// Метод-обработчик, который вызывается после получения данных от YG2.
        /// </summary>
        /// <param name="lb">Данные таблицы лидеров типа LBData.</param>
        private void OnLeaderboardReceived(LBData lb)
        {
            if (lb.technoName != GameConstants.LeaderboardName) return;

            if (_leaderboardView)
            {
                _leaderboardView.BuildLeaderboard(lb);
            }
            else
            {
                Debug.LogError("LeaderboardController не найден в StatisticsWindowManager!", this);
            }
        }

        /// <summary>
        /// Скрывает окно статистики.
        /// </summary>
        private void HideStatisticsWindow()
        {
            if (_statisticsWindow)
            {
                _statisticsWindow.SetActive(false);
            }
        }
    }
}
