using Core;
using Core.Events;
using UnityEngine;
using UnityEngine.UI;
using YandexGames;

namespace View.UI
{
    /// <summary>
    /// Управляет видимостью окна настроек и состоянием его элементов.
    /// </summary>
    public class OptionsWindowManager : MonoBehaviour
    {
        private GameObject _optionsWindow;
        private Toggle _topLineToggle;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            _optionsWindow = transform.Find("Options")?.gameObject;
            if (_optionsWindow != null)
            {
                _topLineToggle = _optionsWindow.transform.FindComponentInChildren<Toggle>("Toggle_TopLine");
                var btnHideOptions = _optionsWindow.transform.FindComponentInChildren<Button>("Closed");
                btnHideOptions?.onClick.AddListener(() => GlobalEvents.OnHideOptions?.Invoke());
                
                var btnNewGame = _optionsWindow.transform.FindComponentInChildren<Button>("NewGame");
                btnNewGame?.onClick.AddListener(() => GlobalEvents.OnRequestNewGame?.Invoke());
                
                var btnHardReset = _optionsWindow.transform.FindComponentInChildren<Button>("HardReset");
                btnHardReset?.onClick.AddListener(() => GlobalEvents.OnRequestHardReset?.Invoke());
            }
        }

        private void OnEnable()
        {
            GlobalEvents.OnShowOptions += ShowOptionsWindow;
            GlobalEvents.OnHideOptions += HideOptionsWindow;

            if (!_topLineToggle) return;
            _topLineToggle.isOn = YandexGamesSdk.Saves.isTopLineVisible;
            _topLineToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void Start()
        {
            if (_optionsWindow)
            {
                _optionsWindow.SetActive(false);
            }
        }

        private void OnDisable()
        {
            GlobalEvents.OnShowOptions -= ShowOptionsWindow;
            GlobalEvents.OnHideOptions -= HideOptionsWindow;

            if (_topLineToggle)
            {
                _topLineToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }

        /// <summary>
        /// Вызывается, когда пользователь изменяет состояние Toggle.
        /// </summary>
        /// <param name="isOn">Новое состояние.</param>
        private void OnToggleValueChanged(bool isOn)
        {
            GlobalEvents.OnToggleTopLine?.Invoke(isOn);
        }

        /// <summary>
        /// Показывает окно настроек.
        /// </summary>
        private void ShowOptionsWindow()
        {
            if (_optionsWindow)
            {
                _optionsWindow.SetActive(true);
            }
        }

        /// <summary>
        /// Скрывает окно настроек.
        /// </summary>
        private void HideOptionsWindow()
        {
            if (_optionsWindow)
            {
                _optionsWindow.SetActive(false);
            }
        }
    }
}
