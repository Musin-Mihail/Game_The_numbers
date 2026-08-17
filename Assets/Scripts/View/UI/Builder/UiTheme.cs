using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;

namespace View.UI.Builder
{
    /// <summary>
    /// Цвета, размеры и шрифты интерфейса. Единственное место визуальной настройки для агента.
    /// Спрайты и шрифты — только <c>Assets/Resources</c>.
    /// </summary>
    public static class UiTheme
    {
        public static readonly Vector2 ReferenceResolution = new(1080, 1920);

        public const float HeaderHeight = 170f;
        public const float ButtonsHeight = 200f;
        public const float HudButtonSize = 150f;
        public const float MenuButtonWidth = 730f;
        public const float MenuButtonHeight = 190f;
        public const float OverlayPanelWidthInset = 350f;
        public const int LineBuffer = 2;
        public const float FloatingScoreLifeTime = 1f;
        public const float FloatingScoreFadeOut = 0.5f;
        public const float LeaderboardFontMultiplier = 1.2f;

        public static readonly Color CanvasBackground = Color.white;
        public static readonly Color OverlaySolid = Color.white;
        public static readonly Color MenuOverlay = OverlaySolid;
        public static readonly Color OverlayDim = OverlaySolid;
        public static readonly Color Panel = new(0.93f, 0.95f, 0.98f, 1f);
        public static readonly Color Button = new(0.53f, 0.70f, 0.86f, 1f);
        public static readonly Color ButtonSecondary = new(0.75f, 0.82f, 0.90f, 1f);
        public static readonly Color TextDark = new(0.1f, 0.12f, 0.16f, 1f);
        public static readonly Color Icon = new(0.1f, 0.12f, 0.16f, 1f);
        public static readonly Color TextMuted = new(0f, 0f, 0f, 0.5f);
        public static readonly Color PositiveScore = new(0.08f, 0.55f, 0.12f, 1f);
        public static readonly Color NegativeScore = Color.red;

        public static readonly Color CellOriginal = new(0.533f, 0.702f, 0.859f, 0.502f);
        public static readonly Color CellHint = new(0.6f, 1f, 0.6f, 1f);
        public static readonly Color CellSelect = new(0.533f, 0.702f, 0.859f, 1f);
        public static readonly Color RulesSameNumber = Color.yellow;
        public static readonly Color RulesSumIsTen = Color.cyan;
        public static readonly Color RulesLineWrap = Color.magenta;
        public static readonly Color RulesFirstAndLast = Color.green;

        public static TMP_FontAsset Font
        {
            get
            {
                if (_font != null) return _font;
                _font = Resources.Load<TMP_FontAsset>("Fonts/light_pixel-7_main")
                        ?? Resources.Load<TMP_FontAsset>("Fonts/NotoSans-Black")
                        ?? TMP_Settings.defaultFontAsset;
                return _font;
            }
        }

        private static TMP_FontAsset _font;
        private static TMP_FontAsset _cjkFont;
        private static Sprite _whiteSprite;
        private static readonly Dictionary<string, Sprite> LanguageSprites = new();

        public static TMP_FontAsset CjkFont
        {
            get
            {
                if (_cjkFont != null) return _cjkFont;
                _cjkFont = Resources.Load<TMP_FontAsset>("Fonts/NotoSansSC-Black") ?? Font;
                return _cjkFont;
            }
        }

        /// <summary>
        /// Название языка на нём самом. Для списка выбора, не зависит от текущего UI-языка.
        /// </summary>
        public static string GetNativeLanguageName(string code)
        {
            return code.ToUpperInvariant() switch
            {
                "EN" => "English",
                "ZH" => "中文",
                "ES" => "Español",
                "FR" => "Français",
                "RU" => "Русский",
                "DE" => "Deutsch",
                "TR" => "Türkçe",
                _ => code
            };
        }

        /// <summary>
        /// Белый спрайт 8x8. Unity Image без спрайта не рисует квад.
        /// </summary>
        public static Sprite WhiteSprite
        {
            get
            {
                if (_whiteSprite != null) return _whiteSprite;
                _whiteSprite = CreateSolidSprite(Color.white);
                return _whiteSprite;
            }
        }

        public static Sprite LoadSprite(params string[] resourcePaths)
        {
            foreach (var path in resourcePaths)
            {
                if (string.IsNullOrEmpty(path)) continue;
                var sprite = Resources.Load<Sprite>(path);
                if (sprite != null) return sprite;
            }

            return WhiteSprite;
        }

        public static Sprite CellSprite => LoadSprite("Sprites/Cell");
        public static Sprite PlayerSprite => LoadSprite("Sprites/player");
        public static Sprite MenuSprite => LoadSprite("Sprites/menu");
        public static Sprite AddSprite => LoadSprite("Sprites/add");
        public static Sprite UndoSprite => LoadSprite("Sprites/undo");
        public static Sprite HintSprite => LoadSprite("Sprites/hint");
        public static Sprite RatingSprite => LoadSprite("Sprites/Rating");
        public static Sprite CloseSprite => LoadSprite("Sprites/Close");
        public static Sprite CheckBoxSprite => LoadSprite("Sprites/CheckBox");

        public static Sprite GetLanguageSprite(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) languageCode = "en";
            var key = languageCode.ToLowerInvariant();
            if (LanguageSprites.TryGetValue(key, out var cached)) return cached;

            var sprite = LoadSprite("Sprites/Lang/" + key.ToUpperInvariant());
            LanguageSprites[key] = sprite;
            return sprite;
        }

        public static Sprite CreateSolidSprite(Color color)
        {
            var texture = new Texture2D(8, 8, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point
            };
            var pixels = new Color[64];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 100f);
        }

        public static string VersionLabel => "v" + GameConstants.GameVersion;
    }
}
