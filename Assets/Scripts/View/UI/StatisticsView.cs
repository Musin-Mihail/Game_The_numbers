using Core;
using Core.Events;
using Localization;
using Model;
using TMPro;
using UnityEngine;
using YG;

namespace View.UI
{
    /// <summary>
    /// Отображает игровую статистику (счет, рекорд и множитель).
    /// </summary>
    public class StatisticsView : MonoBehaviour
    {
        private TextMeshProUGUI _scoreText;
        private TextMeshProUGUI _recordText;
        private TextMeshProUGUI _multiplierText;

        private LocalizationManager _localizationManager;
        private StatisticsModel _statisticsModel;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in allTransforms)
            {
                if (t.name == "Txt_Score") _scoreText = t.GetComponent<TextMeshProUGUI>();
                else if (t.name == "Txt_Record") _recordText = t.GetComponent<TextMeshProUGUI>();
                else if (t.name == "Multiplier") _multiplierText = t.GetComponent<TextMeshProUGUI>();
            }
        }

        private void OnEnable()
        {
            _localizationManager ??= ServiceProvider.GetService<LocalizationManager>();
            _statisticsModel ??= ServiceProvider.GetService<StatisticsModel>();
            GlobalEvents.OnStatisticsChanged += UpdateStatisticsUI;

            LocalizationManager.OnLanguageChanged += HandleLanguageChanged;
            if (_statisticsModel != null)
            {
                UpdateStatisticsUI((_statisticsModel.Score, _statisticsModel.Multiplier));
            }
        }

        private void OnDisable()
        {
            GlobalEvents.OnStatisticsChanged -= UpdateStatisticsUI;
            LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
        }

        /// <summary>
        /// Обработчик события смены языка.
        /// </summary>
        private void HandleLanguageChanged()
        {
            if (_statisticsModel != null)
            {
                UpdateStatisticsUI((_statisticsModel.Score, _statisticsModel.Multiplier));
            }
        }

        /// <summary>
        /// Обновляет UI статистики на основе полученных данных.
        /// </summary>
        private void UpdateStatisticsUI((long score, int multiplier) data)
        {
            if (_localizationManager == null)
            {
                _localizationManager = ServiceProvider.GetService<LocalizationManager>();
                if (_localizationManager == null) return;
            }

            if (_scoreText)
            {
                _scoreText.text = string.Format(_localizationManager.Get("score"), data.score);
            }

            if (_recordText)
            {
                _recordText.text = string.Format(_localizationManager.Get("record"), YG2.saves.record);
            }

            if (_multiplierText)
            {
                _multiplierText.text = string.Format(_localizationManager.Get("multiplier"), data.multiplier);
            }
        }
    }
}