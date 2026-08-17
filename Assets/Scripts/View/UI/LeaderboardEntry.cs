using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.UI.Builder;
using YG.Utils.LB;

namespace View.UI
{
    /// <summary>
    /// Управляет отображением данных одной строки в таблице лидеров.
    /// </summary>
    public class LeaderboardEntry : MonoBehaviour
    {
        private float _fontSizeMultiplier = UiTheme.LeaderboardFontMultiplier;

        private GameObject _playerInfoContainer;
        private TextMeshProUGUI _rankText;
        private Image _photoImage;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _scoreText;
        private GameObject _separatorContainer;

        private float _defaultRankFontSize;
        private float _defaultNameFontSize;
        private float _defaultScoreFontSize;

        private void Awake()
        {
            BindUI();
            if (_rankText) _defaultRankFontSize = _rankText.fontSize;
            if (_nameText) _defaultNameFontSize = _nameText.fontSize;
            if (_scoreText) _defaultScoreFontSize = _scoreText.fontSize;
        }

        private void BindUI()
        {
            _playerInfoContainer = transform.Find("Obj_PlayerInfo")?.gameObject;
            if (_playerInfoContainer != null)
            {
                _rankText = _playerInfoContainer.transform.Find("Txt_Rank")?.GetComponent<TextMeshProUGUI>();
                _photoImage = _playerInfoContainer.transform.Find("Img_Photo")?.GetComponent<Image>();
                _nameText = _playerInfoContainer.transform.Find("Txt_Name")?.GetComponent<TextMeshProUGUI>();
                _scoreText = _playerInfoContainer.transform.Find("Txt_Score")?.GetComponent<TextMeshProUGUI>();
            }
            _separatorContainer = transform.Find("Obj_Separator")?.gameObject;
        }

        /// <summary>
        /// Заполняет UI элементы данными игрока и применяет стили.
        /// </summary>
        /// <param name="playerData">Данные игрока от Yandex Games.</param>
        /// <param name="isCurrentPlayer">True, если это запись текущего игрока.</param>
        public void Populate(LBPlayerData playerData, bool isCurrentPlayer)
        {
            if (_playerInfoContainer != null) _playerInfoContainer.SetActive(true);
            if (_separatorContainer != null) _separatorContainer.SetActive(false);

            if (_rankText != null) _rankText.text = playerData.rank.ToString();
            if (_nameText != null) _nameText.text = playerData.name;
            if (_scoreText != null) _scoreText.text = playerData.score.ToString();

            if (_photoImage && !string.IsNullOrEmpty(playerData.photo))
            {
                StartCoroutine(ImageDownloader.LoadImage(_photoImage, playerData.photo));
            }

            if (isCurrentPlayer)
            {
                if (_rankText) _rankText.fontSize = _defaultRankFontSize * _fontSizeMultiplier;
                if (_nameText) _nameText.fontSize = _defaultNameFontSize * _fontSizeMultiplier;
                if (_scoreText) _scoreText.fontSize = _defaultScoreFontSize * _fontSizeMultiplier;
            }
            else
            {
                if (_rankText) _rankText.fontSize = _defaultRankFontSize;
                if (_nameText) _nameText.fontSize = _defaultNameFontSize;
                if (_scoreText) _scoreText.fontSize = _defaultScoreFontSize;
            }
        }

        /// <summary>
        /// Превращает эту строку в разделитель "---".
        /// </summary>
        public void SetAsSeparator()
        {
            if (_playerInfoContainer != null) _playerInfoContainer.SetActive(false);
            if (_separatorContainer != null) _separatorContainer.SetActive(true);
        }
    }
}