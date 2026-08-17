using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Events;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Builder;

namespace View.UI
{
    /// <summary>
    /// Управляет UI для выбора языка в игре.
    /// </summary>
    public class LanguageSelector : MonoBehaviour
    {
        /// <summary>
        /// Структура для сопоставления кода языка и его иконки в инспекторе.
        /// </summary>
        [Serializable]
        public struct LanguageSpriteMapping
        {
            public string languageCode;
            public Sprite languageSprite;
        }

        private Image _currentLanguageImage;
        private Button _openLanguagePanelButton;
        private GameObject _languagePanel;
        private Button _closeButton;
        private Button _backgroundCloseButton;

        [Header("Настройки языков")]
        [Tooltip("Список сопоставлений кодов языков (например, 'ru', 'en') и их иконок.")]
        [SerializeField] private List<LanguageSpriteMapping> languageMappings;

        private LocalizationManager _localizationManager;
        private readonly Dictionary<string, Sprite> _spriteMap = new();

        private void Awake()
        {
            BindUI();
            LoadLanguageSprites();
        }

        private void LoadLanguageSprites()
        {
            if (languageMappings != null)
            {
                foreach (var mapping in languageMappings.Where(mapping =>
                             mapping.languageSprite != null && !_spriteMap.ContainsKey(mapping.languageCode)))
                {
                    _spriteMap.Add(mapping.languageCode, mapping.languageSprite);
                }
            }

            foreach (var code in UiIds.LanguageButtonNames.Select(n => n.ToLowerInvariant()))
            {
                if (_spriteMap.ContainsKey(code)) continue;
                _spriteMap[code] = UiTheme.GetLanguageSprite(code);
            }
        }

        private void BindUI()
        {
            _openLanguagePanelButton = transform.FindComponentInChildren<Button>("Btn_OpenLanguagePanel");
            _languagePanel = transform.FindGameObjectInChildren("Obj_LanguagePanel");
            if (_openLanguagePanelButton != null) _currentLanguageImage = transform.FindComponentInChildren<Image>("Img_CurrentLanguage");
            if (_languagePanel != null)
            {
                _closeButton = transform.FindComponentInChildren<Button>("Btn_Close");
                _backgroundCloseButton = transform.FindComponentInChildren<Button>("Btn_BackgroundClose");
                var languagesParent = transform.FindGameObjectInChildren("Languages")?.transform;
                if (languagesParent != null)
                {
                    foreach (Transform child in languagesParent)
                    {
                        var btn = child.GetComponent<Button>();
                        var langCode = child.name.ToLower();
                        if (btn != null)
                        {
                            btn.onClick.AddListener(() =>
                            {
                                GlobalEvents.OnSetLanguage?.Invoke(langCode);
                                HidePanel();
                            });
                        }
                    }
                }
            }
            /*
            {
                if (t.name == "Btn_OpenLanguagePanel") _openLanguagePanelButton = t.GetComponent<Button>();
                else if (t.name == "Obj_LanguagePanel") _languagePanel = t.gameObject;
            }

            if (_openLanguagePanelButton != null) 
                _currentLanguageImage = _openLanguagePanelButton.transform.Find("Img_CurrentLanguage")?.GetComponent<Image>();

            if (_languagePanel != null)
            {
                var panel = _languagePanel.transform.Find("Panel");
                if (panel != null) _closeButton = panel.Find("Btn_Close")?.GetComponent<Button>();
                _backgroundCloseButton = _languagePanel.transform.Find("Btn_BackgroundClose")?.GetComponent<Button>();
                
                // Авто-подписка кнопок смены языка (EN, RU и т.д.)
                var languagesParent = panel?.Find("Languages");
                if (languagesParent != null)
                {
                    foreach (Transform child in languagesParent)
                    {
                        var btn = child.GetComponent<Button>();
                        var langCode = child.name.ToLower();
            */
        }

        private void Start()
        {
            _localizationManager = ServiceProvider.GetService<LocalizationManager>();

            if (_localizationManager == null)
            {
                Debug.LogError("LanguageSelector не смог получить LocalizationManager. Компонент будет отключен.", this);
                enabled = false;
                return;
            }

            if (_openLanguagePanelButton != null) _openLanguagePanelButton.onClick.AddListener(ShowPanel);
            if (_closeButton != null) _closeButton.onClick.AddListener(HidePanel);
            if (_backgroundCloseButton != null) _backgroundCloseButton.onClick.AddListener(HidePanel);
            if (_languagePanel != null) _languagePanel.SetActive(false);
            UpdateCurrentLanguageIcon();
        }

        private void OnEnable()
        {
            LocalizationManager.OnLanguageChanged += UpdateCurrentLanguageIcon;
            UpdateCurrentLanguageIcon();
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= UpdateCurrentLanguageIcon;
        }

        /// <summary>
        /// Показывает панель выбора языка.
        /// </summary>
        private void ShowPanel()
        {
            if (_languagePanel != null) _languagePanel.SetActive(true);
        }

        /// <summary>
        /// Скрывает панель выбора языка.
        /// </summary>
        private void HidePanel()
        {
            if (_languagePanel != null) _languagePanel.SetActive(false);
        }

        /// <summary>
        /// Вызывается при нажатии на кнопку выбора языка. Этот метод нужно вызывать из инспектора Unity.
        /// </summary>
        /// <param name="languageCode">Код выбранного языка (e.g., "ru", "en").</param>
        public void SelectLanguage(string languageCode)
        {
            GlobalEvents.OnSetLanguage?.Invoke(languageCode);
            HidePanel();
        }

        /// <summary>
        /// Обновляет иконку в углу экрана в соответствии с текущим языком.
        /// </summary>
        private void UpdateCurrentLanguageIcon()
        {
            if (_localizationManager == null || _currentLanguageImage == null) return;

            var currentLang = _localizationManager.CurrentLanguage ?? "en";
            if (_spriteMap.TryGetValue(currentLang, out var sprite))
            {
                _currentLanguageImage.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"Спрайт для языка '{currentLang}' не найден.", this);
            }
        }

        private void OnDestroy()
        {
            if (_openLanguagePanelButton) _openLanguagePanelButton.onClick.RemoveAllListeners();
            if (_closeButton) _closeButton.onClick.RemoveAllListeners();
            if (_backgroundCloseButton) _backgroundCloseButton.onClick.RemoveAllListeners();
        }
    }
}