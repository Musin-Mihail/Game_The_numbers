using Core;
using Core.Events;
using UnityEngine;

namespace View.UI
{
    /// <summary>
    /// Управляет видимостью окна с правилами игры.
    /// </summary>
    public class RulesWindowManager : MonoBehaviour
    {
        private GameObject _rulesWindow;
        private RulesGrid _rulesGrid;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            _rulesWindow = transform.Find("Rules")?.gameObject;
            if (_rulesWindow != null)
            {
                _rulesGrid = _rulesWindow.transform.FindComponentInChildren<RulesGrid>("RulesGrid") ?? UnityEngine.Object.FindFirstObjectByType<RulesGrid>(FindObjectsInactive.Include);
                
                var btnHideRules = _rulesWindow.transform.FindComponentInChildren<UnityEngine.UI.Button>("Closed");
                btnHideRules?.onClick.AddListener(() => GlobalEvents.OnHideRules?.Invoke());
            }
        }

        private void OnEnable()
        {
            GlobalEvents.OnShowRules += ShowRulesWindow;
            GlobalEvents.OnHideRules += HideRulesWindow;
        }

        private void Start()
        {
            if (_rulesWindow)
            {
                _rulesWindow.SetActive(false);
            }
        }

        private void OnDisable()
        {
            GlobalEvents.OnShowRules -= ShowRulesWindow;
            GlobalEvents.OnHideRules -= HideRulesWindow;
        }

        /// <summary>
        /// Показывает окно правил и генерирует демонстрационную сетку.
        /// </summary>
        private void ShowRulesWindow()
        {
            if (!_rulesWindow) return;
            _rulesWindow.SetActive(true);
            if (_rulesGrid)
            {
                _rulesGrid.GenerateGrid();
            }
            else
            {
                Debug.LogError("RulesGrid не найден в RulesWindowManager.");
            }
        }

        /// <summary>
        /// Скрывает окно правил.
        /// </summary>
        private void HideRulesWindow()
        {
            if (_rulesWindow)
            {
                _rulesWindow.SetActive(false);
            }
        }
    }
}
