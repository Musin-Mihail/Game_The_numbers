using System;
using System.Collections;
using TMPro;
using UnityEngine;
using View.UI.Builder;

namespace View.UI
{
    /// <summary>
    /// Представляет собой "всплывающий" текст (например, очки), который появляется и исчезает.
    /// </summary>
    public class FloatingScore : MonoBehaviour
    {
        private TextMeshProUGUI _scoreText;
        private RectTransform _rectTransform;
        private Action<FloatingScore> _onComplete;
        private readonly float _fadeOutTime = UiTheme.FloatingScoreFadeOut;
        private readonly float _lifeTime = UiTheme.FloatingScoreLifeTime;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            BindUI();
        }

        private void BindUI()
        {
            _scoreText = transform.Find("Txt_ScoreText")?.GetComponent<TextMeshProUGUI>();
        }

        /// <summary>
        /// Показывает и анимирует всплывающий текст.
        /// </summary>
        /// <param name="text">Отображаемый текст.</param>
        /// <param name="color">Цвет текста.</param>
        /// <param name="centerPosition">Позиция центра текста.</param>
        /// <param name="size">Размер RectTransform.</param>
        /// <param name="onComplete">Callback, вызываемый по завершении анимации.</param>
        public void Show(string text, Color color, Vector2 centerPosition, Vector2 size, Action<FloatingScore> onComplete)
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            if (_scoreText == null) BindUI();

            _onComplete = onComplete;
            if (_scoreText != null)
            {
                _scoreText.text = text;
                _scoreText.color = color;
            }

            _rectTransform.sizeDelta = size;
            _rectTransform.anchoredPosition = centerPosition;
            transform.SetAsLastSibling();
            if (transform.parent != null) transform.parent.SetAsLastSibling();

            gameObject.SetActive(true);
            if (gameObject.activeInHierarchy) StartCoroutine(Animate());
            else _onComplete?.Invoke(this);
        }

        private IEnumerator Animate()
        {
            var elapsedTime = 0f;
            var startColor = _scoreText != null ? _scoreText.color : Color.white;

            while (elapsedTime < _lifeTime)
            {
                if (elapsedTime > _lifeTime - _fadeOutTime)
                {
                    var alpha = Mathf.Lerp(1f, 0f, (elapsedTime - (_lifeTime - _fadeOutTime)) / _fadeOutTime);
                    if (_scoreText != null) _scoreText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (_scoreText != null) _scoreText.color = startColor;
            _onComplete?.Invoke(this);
        }
    }
}