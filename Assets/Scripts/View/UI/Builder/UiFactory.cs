using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View.UI.Builder
{
    /// <summary>
    /// Примитивы uGUI. Не содержит игровых экранов — только сборка объектов.
    /// </summary>
    public static class UiFactory
    {
        public const int UiLayer = 5;

        public static GameObject Create(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = UiLayer;
            go.transform.SetParent(parent, false);
            return go;
        }

        public static RectTransform Rect(GameObject go) => (RectTransform)go.transform;

        public static void Stretch(RectTransform rt, Vector2 offsetMin, Vector2 offsetMax)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
        }

        public static void Stretch(RectTransform rt) => Stretch(rt, Vector2.zero, Vector2.zero);

        public static void SetRect(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        public static Image AddImage(GameObject go, Color color, Sprite sprite = null, bool raycast = true)
        {
            if (go.GetComponent<CanvasRenderer>() == null) go.AddComponent<CanvasRenderer>();
            var image = go.GetComponent<Image>() ?? go.AddComponent<Image>();
            image.color = color;
            var used = sprite != null ? sprite : UiTheme.WhiteSprite;
            image.sprite = used;
            image.type = used.border.sqrMagnitude > 0f ? Image.Type.Sliced : Image.Type.Simple;
            image.raycastTarget = raycast;
            return image;
        }

        public static Button AddButton(GameObject go)
        {
            var button = go.GetComponent<Button>() ?? go.AddComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            return button;
        }

        public static TextMeshProUGUI AddText(GameObject go, string text, float fontSize, Color color,
            TextAlignmentOptions alignment, bool raycast = false, bool autoSize = false)
        {
            if (go.GetComponent<CanvasRenderer>() == null) go.AddComponent<CanvasRenderer>();
            var tmp = go.GetComponent<TextMeshProUGUI>() ?? go.AddComponent<TextMeshProUGUI>();
            tmp.font = UiTheme.Font;
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.raycastTarget = raycast;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            if (autoSize)
            {
                tmp.enableAutoSizing = true;
                tmp.fontSizeMin = 10f;
                tmp.fontSizeMax = fontSize;
            }

            return tmp;
        }

        public static TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize,
            Color color, TextAlignmentOptions alignment, bool autoSize = false)
        {
            var go = Create(name, parent);
            Stretch(Rect(go));
            return AddText(go, text, fontSize, color, alignment, false, autoSize);
        }

        public static LocalizableText BindLocalization(GameObject textObject, string key)
        {
            var loc = textObject.GetComponent<LocalizableText>() ?? textObject.AddComponent<LocalizableText>();
            loc.Bind(key);
            return loc;
        }

        public static GameObject CreateLabeledButton(string name, Transform parent, string localizationKey,
            Vector2 size)
        {
            var go = Create(name, parent);
            SetRect(Rect(go), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f),
                Vector2.zero, size);
            AddImage(go, UiTheme.Button);
            AddButton(go);
            var label = CreateText("Text (TMP)", go.transform, localizationKey, 48f, UiTheme.TextDark,
                TextAlignmentOptions.Center, true);
            BindLocalization(label.gameObject, localizationKey);
            return go;
        }

        public static GameObject CreateHudIconButton(string name, Transform parent, string label, Sprite icon = null)
        {
            var go = Create(name, parent);
            SetRect(Rect(go), Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f), Vector2.zero,
                new Vector2(UiTheme.HudButtonSize, UiTheme.HudButtonSize));
            var color = icon != null ? UiTheme.Icon : UiTheme.Button;
            var image = AddImage(go, color, icon);
            image.preserveAspect = icon != null;
            AddButton(go);
            if (icon == null && !string.IsNullOrEmpty(label))
            {
                CreateText("Label", go.transform, label, 42f, UiTheme.TextDark, TextAlignmentOptions.Center, true);
            }

            return go;
        }

        public static GameObject CreateStretchOverlay(string name, Transform parent, Color background)
        {
            var go = Create(name, parent);
            Stretch(Rect(go));
            var bg = Create(UiIds.Background, go.transform);
            Stretch(Rect(bg));
            AddImage(bg, background);
            return go;
        }
    }
}
