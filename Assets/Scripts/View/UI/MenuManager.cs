using Core;
using Core.Events;
using UnityEngine;

namespace View.UI
{
    /// <summary>
    /// Управляет видимостью главного меню.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        private GameObject _windowMenu;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            _windowMenu = transform.Find("Menu")?.gameObject;
            if (_windowMenu != null)
            {
                var btnHideMenu = _windowMenu.transform.FindComponentInChildren<UnityEngine.UI.Button>("Continue");
                btnHideMenu?.onClick.AddListener(() => GlobalEvents.OnHideMenu?.Invoke());

                var btnShowOptions = _windowMenu.transform.FindComponentInChildren<UnityEngine.UI.Button>("Options");
                btnShowOptions?.onClick.AddListener(() => {
                    GlobalEvents.OnShowOptions?.Invoke();
                    GlobalEvents.OnRequestMarkUpdateSeen?.Invoke();
                });

                var btnShowRules = _windowMenu.transform.FindComponentInChildren<UnityEngine.UI.Button>("Rules");
                btnShowRules?.onClick.AddListener(() => GlobalEvents.OnShowRules?.Invoke());
            }
        }

        private void OnEnable()
        {
            GlobalEvents.OnShowMenu += ShowMenu;
            GlobalEvents.OnHideMenu += HideMenu;
        }

        private void Start()
        {
            if (_windowMenu)
            {
                _windowMenu.SetActive(true);
            }
        }

        private void OnDisable()
        {
            GlobalEvents.OnShowMenu -= ShowMenu;
            GlobalEvents.OnHideMenu -= HideMenu;
        }

        /// <summary>
        /// Показывает окно меню.
        /// </summary>
        private void ShowMenu()
        {
            if (_windowMenu)
            {
                _windowMenu.SetActive(true);
            }
        }

        /// <summary>
        /// Скрывает окно меню.
        /// </summary>
        private void HideMenu()
        {
            if (_windowMenu)
            {
                _windowMenu.SetActive(false);
            }
        }
    }
}
