using Core.Events;
using UnityEngine;
using UnityEngine.UI;
using YG;

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
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in allTransforms)
            {
                if (t.name == "Options" && t.parent != null && t.parent.name == "Canvas") _optionsWindow = t.gameObject;
                else if (t.name == "Toggle_TopLine") _topLineToggle = t.GetComponent<Toggle>();
            }

            if (_optionsWindow != null)
            {
                var btnHideOptions = _optionsWindow.transform.Find("Obj_OptionsWindow/Closed")?.GetComponent<Button>();
                btnHideOptions?.onClick.AddListener(() => GlobalEvents.OnHideOptions?.Invoke());

                var btnNewGame = _optionsWindow.transform.Find("Obj_OptionsWindow/NewGame")?.GetComponent<Button>();
                btnNewGame?.onClick.AddListener(() => GlobalEvents.OnRequestNewGame?.Invoke());

                var btnHardReset = _optionsWindow.transform.Find("Obj_OptionsWindow/HardReset")?.GetComponent<Button>();
                btnHardReset?.onClick.AddListener(() => GlobalEvents.OnRequestHardReset?.Invoke());
            }
        }

        private void OnEnable()
        {
            GlobalEvents.OnShowOptions += ShowOptionsWindow;
            GlobalEvents.OnHideOptions += HideOptionsWindow;

            if (!_topLineToggle) return;
            _topLineToggle.isOn = YG2.saves.isTopLineVisible;
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