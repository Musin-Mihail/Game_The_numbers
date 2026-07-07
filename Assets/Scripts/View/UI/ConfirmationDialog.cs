using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI
{
    /// <summary>
    /// Управляет модальным окном для подтверждения действий пользователя.
    /// </summary>
    public class ConfirmationDialog : MonoBehaviour
    {
        private TextMeshProUGUI _messageText;
        private Button _yesButton;
        private Button _noButton;
        private TextMeshProUGUI _yesButtonText;
        private TextMeshProUGUI _noButtonText;
        private RectTransform _panel;

        private Action _onYesAction;
        private Action _onNoAction;

        private void Awake()
        {
            BindUI();
            if (_yesButton != null) _yesButton.onClick.AddListener(OnYesClicked);
            if (_noButton != null) _noButton.onClick.AddListener(OnNoClicked);
        }

        private void BindUI()
        {
            _panel = transform.Find("Panel")?.GetComponent<RectTransform>();
            if (_panel != null)
            {
                _messageText = _panel.Find("Txt_Message")?.GetComponent<TextMeshProUGUI>();
                _yesButton = _panel.Find("Btn_Yes")?.GetComponent<Button>();
                if (_yesButton != null) _yesButtonText = _yesButton.transform.Find("Txt_YesButton")?.GetComponent<TextMeshProUGUI>();
                
                _noButton = _panel.Find("Btn_No")?.GetComponent<Button>();
                if (_noButton != null) _noButtonText = _noButton.transform.Find("Txt_NoButton")?.GetComponent<TextMeshProUGUI>();
            }
        }

        /// <summary>
        /// Показывает диалоговое окно с заданным сообщением и действиями.
        /// </summary>
        /// <param name="message">Сообщение для пользователя.</param>
        /// <param name="yesText">Текст для кнопки подтверждения.</param>
        /// <param name="noText">Текст для кнопки отмены.</param>
        /// <param name="onYes">Действие, выполняемое при нажатии "Да".</param>
        /// <param name="onNo">Действие, выполняемое при нажатии "Нет".</param>
        /// <param name="newSize">Новый размер панели диалога.</param>
        public void Show(string message, string yesText, string noText, Action onYes, Action onNo, Vector2 newSize)
        {
            if (_panel != null) _panel.sizeDelta = newSize;
            if (_messageText != null) _messageText.text = message;

            if (_yesButtonText) _yesButtonText.text = yesText;
            if (_noButtonText) _noButtonText.text = noText;

            _onYesAction = onYes;
            _onNoAction = onNo;

            if (_yesButton != null) _yesButton.gameObject.SetActive(!string.IsNullOrEmpty(yesText));
            if (_noButton != null) _noButton.gameObject.SetActive(!string.IsNullOrEmpty(noText));

            gameObject.SetActive(true);
        }

        private void OnYesClicked()
        {
            _onYesAction?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnNoClicked()
        {
            _onNoAction?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_yesButton != null) _yesButton.onClick.RemoveListener(OnYesClicked);
            if (_noButton != null) _noButton.onClick.RemoveListener(OnNoClicked);
        }
    }
}