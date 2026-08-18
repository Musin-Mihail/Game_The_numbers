using Core.Events;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Builder;

namespace View.UI
{
    /// <summary>
    /// Текст обучения под сеткой и кнопка «Продолжить» на шаге с очками.
    /// </summary>
    public class TutorialCaptionView : MonoBehaviour
    {
        private LocalizableText _captionText;
        private GameObject _continueButton;

        private void Awake()
        {
            var textGo = transform.Find(UiIds.TutorialCaptionText)?.gameObject;
            if (textGo != null)
            {
                _captionText = textGo.GetComponent<LocalizableText>() ?? textGo.AddComponent<LocalizableText>();
            }

            _continueButton = transform.Find(UiIds.TutorialContinue)?.gameObject;
            var button = _continueButton?.GetComponent<Button>();
            button?.onClick.AddListener(() => GlobalEvents.OnTutorialContinue?.Invoke());

            GlobalEvents.OnTutorialCaptionChanged += HandleCaptionChanged;
            GlobalEvents.OnTutorialCompleted += Hide;
        }

        private void OnDestroy()
        {
            GlobalEvents.OnTutorialCaptionChanged -= HandleCaptionChanged;
            GlobalEvents.OnTutorialCompleted -= Hide;
        }

        private void HandleCaptionChanged((string localizationKey, bool showContinue) data)
        {
            if (string.IsNullOrEmpty(data.localizationKey))
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);
            _captionText?.Bind(data.localizationKey);
            if (_continueButton)
            {
                _continueButton.SetActive(data.showContinue);
            }
        }

        private void Hide()
        {
            if (_continueButton)
            {
                _continueButton.SetActive(false);
            }

            gameObject.SetActive(false);
        }
    }
}
