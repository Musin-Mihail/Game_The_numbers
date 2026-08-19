using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View.Grid;

namespace View.UI.Builder
{
    /// <summary>
    /// Создаёт ячейку, всплывающие очки и строку лидерборда вместо YAML-префабов.
    /// </summary>
    public static class WidgetFactory
    {
        public static Cell CreateCell(Transform parent)
        {
            var go = UiFactory.Create("Prefab_Cell", parent);
            var rt = UiFactory.Rect(go);
            UiFactory.SetRect(rt, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                Vector2.zero, new Vector2(GameConstants.CellSize, GameConstants.CellSize));

            var cellImage = UiFactory.AddImage(go, UiTheme.CellOriginal, UiTheme.CellSprite);
            cellImage.preserveAspect = true;
            go.AddComponent<CellAnimator>();
            var button = UiFactory.AddButton(go);

            var numberGo = UiFactory.Create(UiIds.CellNumber, go.transform);
            UiFactory.Stretch(UiFactory.Rect(numberGo), new Vector2(4f, 4f), new Vector2(-4f, -4f));
            var number = UiFactory.AddText(numberGo, "0", UiTheme.CellNumberFontSize, Color.black,
                TextAlignmentOptions.Center);
            number.enableAutoSizing = false;
            number.textWrappingMode = TextWrappingModes.NoWrap;
            number.overflowMode = TextOverflowModes.Overflow;
            number.margin = UiTheme.CellNumberMargin;

            var cell = go.AddComponent<Cell>();
            button.onClick.AddListener(cell.HandleClick);
            return cell;
        }

        public static FloatingScore CreateFloatingScore(Transform parent)
        {
            var go = UiFactory.Create("FloatingScore", parent);
            var rt = UiFactory.Rect(go);
            UiFactory.SetRect(rt, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(107f, 107f));
            var ignoreLayout = go.AddComponent<LayoutElement>();
            ignoreLayout.ignoreLayout = true;
            var textGo = UiFactory.Create(UiIds.FloatingScoreText, go.transform);
            UiFactory.Stretch(UiFactory.Rect(textGo));
            UiFactory.AddText(textGo, "0", 64f, UiTheme.PositiveScore, TextAlignmentOptions.Center, false, true);
            return go.AddComponent<FloatingScore>();
        }

        public static LeaderboardEntry CreateLeaderboardEntry(Transform parent)
        {
            var go = UiFactory.Create("LeaderboardEntry", parent);
            var rt = UiFactory.Rect(go);
            rt.sizeDelta = new Vector2(1000f, 70f);

            var info = UiFactory.Create(UiIds.LeaderboardInfo, go.transform);
            UiFactory.SetRect(UiFactory.Rect(info), new Vector2(0f, 0.5f), new Vector2(1f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0f, 70f));
            var row = info.AddComponent<HorizontalLayoutGroup>();
            row.childAlignment = TextAnchor.MiddleCenter;
            row.spacing = 10f;
            row.childForceExpandWidth = true;
            row.childForceExpandHeight = true;
            row.childControlWidth = false;
            row.childControlHeight = false;

            CreateEntryText(UiIds.LeaderboardRank, info.transform, 80f);
            var photo = UiFactory.Create(UiIds.LeaderboardPhoto, info.transform);
            photo.AddComponent<LayoutElement>().preferredWidth = 64f;
            UiFactory.SetRect(UiFactory.Rect(photo), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(64f, 64f));
            UiFactory.AddImage(photo, Color.white, UiTheme.PlayerSprite);
            CreateEntryText(UiIds.LeaderboardName, info.transform, 400f);
            CreateEntryText(UiIds.LeaderboardScore, info.transform, 200f);

            var separator = UiFactory.Create(UiIds.LeaderboardSeparator, go.transform);
            UiFactory.Stretch(UiFactory.Rect(separator));
            UiFactory.AddText(separator, "---", 36f, UiTheme.TextMuted, TextAlignmentOptions.Center);
            separator.SetActive(false);

            return go.AddComponent<LeaderboardEntry>();
        }

        private static void CreateEntryText(string name, Transform parent, float width)
        {
            var go = UiFactory.Create(name, parent);
            go.AddComponent<LayoutElement>().preferredWidth = width;
            UiFactory.SetRect(UiFactory.Rect(go), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(width, 70f));
            UiFactory.AddText(go, "", 32f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);
        }
    }
}
