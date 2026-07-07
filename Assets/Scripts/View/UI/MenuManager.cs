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
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in allTransforms)
            {
                // Находим КОРНЕВОЙ объект меню, чтобы включать его вместе с фоном
                if (t.name == "Menu" && t.parent != null && t.parent.name == "Canvas") _windowMenu = t.gameObject;
            }

            if (_windowMenu != null)
            {
                var btnHideMenu = _windowMenu.transform.Find("Obj_WindowMenu/Continue")?.GetComponent<UnityEngine.UI.Button>();
                btnHideMenu?.onClick.AddListener(() => GlobalEvents.OnHideMenu?.Invoke());

                var btnShowOptions = _windowMenu.transform.Find("Obj_WindowMenu/Options")?.GetComponent<UnityEngine.UI.Button>();
                btnShowOptions?.onClick.AddListener(() => {
                    GlobalEvents.OnShowOptions?.Invoke();
                    GlobalEvents.OnRequestMarkUpdateSeen?.Invoke();
                });

                var btnShowRules = _windowMenu.transform.Find("Obj_WindowMenu/Rules")?.GetComponent<UnityEngine.UI.Button>();
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