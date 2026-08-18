using Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using View.Grid;
using View.UI;

namespace View.UI.Builder
{
    /// <summary>
    /// Собирает игровой интерфейс при старте. Источник правды вместо PlayingField.unity Canvas.
    /// </summary>
    public static class PlayingFieldUiBuilder
    {
        public static void ReplaceSceneUi()
        {
            RemoveLegacyUi();
            Build();
        }

        private static void RemoveLegacyUi()
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == UiIds.Canvas)
                {
                    Object.DestroyImmediate(root);
                    continue;
                }

                if (root.name != "Game") continue;
                var ui = root.transform.Find("UI");
                var gameplay = root.transform.Find("GAMEPLAY");
                if (ui != null) Object.DestroyImmediate(ui.gameObject);
                if (gameplay != null) Object.DestroyImmediate(gameplay.gameObject);
            }
        }

        private static void Build()
        {
            var camera = Camera.main;
            var canvasGo = UiFactory.Create(UiIds.Canvas, null);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 100f;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1
                                              | AdditionalCanvasShaderChannels.Normal
                                              | AdditionalCanvasShaderChannels.Tangent;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = UiTheme.ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;

            canvasGo.AddComponent<GraphicRaycaster>();

            BuildHud(canvasGo.transform);
            BuildMenu(canvasGo.transform);
            BuildOptions(canvasGo.transform);
            BuildStatistics(canvasGo.transform);
            BuildLoading(canvasGo.transform);
            BuildConfirmation(canvasGo.transform);
            BuildLanguagePanel(canvasGo.transform);
            BuildVersion(canvasGo.transform);
            BuildGameplayServices();

            canvasGo.AddComponent<LanguageSelector>();
            canvasGo.AddComponent<StatisticsView>();
            canvasGo.AddComponent<MenuManager>();
            canvasGo.AddComponent<Core.Shop.ShopManager>();
            canvasGo.AddComponent<OptionsWindowManager>();
            canvasGo.AddComponent<LeaderboardView>();
            canvasGo.AddComponent<StatisticsWindowManager>();
        }

        private static void BuildHud(Transform canvas)
        {
            var hud = UiFactory.Create(UiIds.HudRoot, canvas);
            UiFactory.Stretch(UiFactory.Rect(hud));

            var background = UiFactory.Create(UiIds.Background, hud.transform);
            UiFactory.Stretch(UiFactory.Rect(background));
            UiFactory.AddImage(background, UiTheme.CanvasBackground, raycast: false);

            var header = UiFactory.Create(UiIds.Header, hud.transform);
            UiFactory.SetRect(UiFactory.Rect(header), new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, UiTheme.HeaderHeight));

            var record = UiFactory.Create(UiIds.Record, header.transform);
            UiFactory.SetRect(UiFactory.Rect(record), new Vector2(0f, 0.5f), new Vector2(0.7f, 1f),
                new Vector2(0f, 1f), new Vector2(24f, -8f), Vector2.zero);
            UiFactory.AddText(record, "", 42f, UiTheme.TextDark, TextAlignmentOptions.MidlineLeft, false, true);

            var score = UiFactory.Create(UiIds.Score, header.transform);
            UiFactory.SetRect(UiFactory.Rect(score), new Vector2(0f, 0f), new Vector2(0.7f, 0.55f),
                new Vector2(0f, 0f), new Vector2(24f, 8f), Vector2.zero);
            UiFactory.AddText(score, "", 48f, UiTheme.TextDark, TextAlignmentOptions.MidlineLeft, false, true);

            var rating = UiFactory.Create(UiIds.Rating, header.transform);
            UiFactory.SetRect(UiFactory.Rect(rating), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(1f, 1f), new Vector2(-10f, -10f), new Vector2(150f, 150f));
            var ratingImage = UiFactory.AddImage(rating, UiTheme.Icon, UiTheme.RatingSprite);
            ratingImage.preserveAspect = true;
            UiFactory.AddButton(rating).onClick.AddListener(() => Core.Events.GlobalEvents.OnShowStatistics?.Invoke());

            var gameSpace = UiFactory.Create(UiIds.GameSpace, hud.transform);
            UiFactory.Stretch(UiFactory.Rect(gameSpace),
                new Vector2(0f, UiTheme.ButtonsHeight),
                new Vector2(0f, -UiTheme.HeaderHeight));

            BuildScrollView(gameSpace.transform);
            BuildTutorialCaption(gameSpace.transform);

            var topLine = UiFactory.Create(UiIds.HeaderContainer, gameSpace.transform);
            UiFactory.SetRect(UiFactory.Rect(topLine), new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, GameConstants.CellSize + GameConstants.Indent));

            var buttons = UiFactory.Create(UiIds.Buttons, hud.transform);
            UiFactory.SetRect(UiFactory.Rect(buttons), new Vector2(0f, 0f), new Vector2(1f, 0f),
                new Vector2(0.5f, 0f), new Vector2(0f, 0f), new Vector2(0f, UiTheme.ButtonsHeight));
            var layout = buttons.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.padding = new RectOffset(0, 0, 27, 0);
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            UiFactory.CreateHudIconButton(UiIds.MenuButton, buttons.transform, "", UiTheme.MenuSprite);
            var add = UiFactory.CreateHudIconButton(UiIds.NewLines, buttons.transform, "", UiTheme.AddSprite);
            add.AddComponent<ButtonAnimator>();
            BuildCounterButton(UiIds.UndoCount, UiIds.UndoImage, UiIds.UndoCountText, buttons.transform,
                UiTheme.UndoSprite);
            BuildCounterButton(UiIds.HintCount, UiIds.HintImage, UiIds.HintCountText, buttons.transform,
                UiTheme.HintSprite);

            hud.AddComponent<ActionCountersView>();
        }

        private static void BuildScrollView(Transform gameSpace)
        {
            var scrollGo = UiFactory.Create(UiIds.ScrollView, gameSpace);
            UiFactory.Stretch(UiFactory.Rect(scrollGo), Vector2.zero, new Vector2(0f, -(GameConstants.CellSize + GameConstants.Indent)));
            UiFactory.AddImage(scrollGo, new Color(1f, 1f, 1f, 0f), raycast: false);

            var viewport = UiFactory.Create(UiIds.Viewport, scrollGo.transform);
            UiFactory.Stretch(UiFactory.Rect(viewport), new Vector2(0f, 0f), new Vector2(-20f, 0f));
            UiFactory.AddImage(viewport, Color.white, raycast: true);
            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            var content = UiFactory.Create(UiIds.Content, viewport.transform);
            UiFactory.SetRect(UiFactory.Rect(content), new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, 1f), Vector2.zero, new Vector2(0f, 0f));

            var scoreHost = UiFactory.Create(UiIds.FloatingScoreHost, content.transform);
            var scoreRt = UiFactory.Rect(scoreHost);
            scoreRt.anchorMin = Vector2.zero;
            scoreRt.anchorMax = Vector2.one;
            scoreRt.pivot = new Vector2(0f, 1f);
            scoreRt.offsetMin = Vector2.zero;
            scoreRt.offsetMax = Vector2.zero;

            var scrollbarGo = UiFactory.Create("Scrollbar Vertical", scrollGo.transform);
            UiFactory.SetRect(UiFactory.Rect(scrollbarGo), new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(1f, 0.5f), Vector2.zero, new Vector2(20f, 0f));
            UiFactory.AddImage(scrollbarGo, UiTheme.ButtonSecondary);
            var sliding = UiFactory.Create("Sliding Area", scrollbarGo.transform);
            UiFactory.Stretch(UiFactory.Rect(sliding), new Vector2(4f, 4f), new Vector2(-4f, -4f));
            var handle = UiFactory.Create("Handle", sliding.transform);
            UiFactory.Stretch(UiFactory.Rect(handle));
            var handleImage = UiFactory.AddImage(handle, UiTheme.Button);

            var scrollbar = scrollbarGo.AddComponent<Scrollbar>();
            scrollbar.handleRect = UiFactory.Rect(handle);
            scrollbar.targetGraphic = handleImage;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            var scroll = scrollGo.AddComponent<ScrollRect>();
            scroll.content = UiFactory.Rect(content);
            scroll.viewport = UiFactory.Rect(viewport);
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.inertia = true;
            scroll.scrollSensitivity = 1f;
            scroll.verticalScrollbar = scrollbar;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        }

        private static void BuildCounterButton(string rootName, string imageName, string textName, Transform parent,
            Sprite icon)
        {
            var go = UiFactory.CreateHudIconButton(rootName, parent, "", icon);
            var image = UiFactory.Create(imageName, go.transform);
            UiFactory.Stretch(UiFactory.Rect(image), new Vector2(8f, 8f), new Vector2(-8f, -8f));

            var text = UiFactory.Create(textName, image.transform);
            UiFactory.SetRect(UiFactory.Rect(text), new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(1f, 0f), new Vector2(4f, 4f), new Vector2(56f, 36f));
            UiFactory.AddText(text, "5", 28f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);
        }

        private static void BuildMenu(Transform canvas)
        {
            var menu = UiFactory.CreateStretchOverlay(UiIds.Menu, canvas, UiTheme.MenuOverlay);
            var window = CreateCenteredWindow(UiIds.WindowMenu, menu.transform, UiTheme.MenuButtonWidth, 40f);
            UiFactory.CreateLabeledButton(UiIds.Continue, window.transform, "play",
                new Vector2(UiTheme.MenuButtonWidth, UiTheme.MenuButtonHeight));
            var options = UiFactory.CreateLabeledButton(UiIds.OptionsButton, window.transform, "options",
                new Vector2(UiTheme.MenuButtonWidth, UiTheme.MenuButtonHeight));
            options.AddComponent<ButtonAnimator>().ConfigureForUpdateNotification();

            var lang = UiFactory.Create(UiIds.OpenLanguage, menu.transform);
            UiFactory.SetRect(UiFactory.Rect(lang), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(1f, 1f), new Vector2(-24f, -24f), new Vector2(120f, 80f));
            UiFactory.AddImage(lang, UiTheme.ButtonSecondary);
            UiFactory.AddButton(lang);
            var flag = UiFactory.Create(UiIds.CurrentLanguageImage, lang.transform);
            UiFactory.Stretch(UiFactory.Rect(flag), new Vector2(8f, 8f), new Vector2(-8f, -8f));
            var flagImage = UiFactory.AddImage(flag, Color.white, UiTheme.GetLanguageSprite("en"));
            flagImage.preserveAspect = true;
        }

        private static void BuildOptions(Transform canvas)
        {
            var options = UiFactory.CreateStretchOverlay(UiIds.Options, canvas, UiTheme.OverlayDim);
            var window = CreateCenteredWindow(UiIds.WindowOptions, options.transform, UiTheme.MenuButtonWidth, 24f);

            var buttonSize = new Vector2(UiTheme.MenuButtonWidth, 140f);
            UiFactory.CreateLabeledButton(UiIds.Closed, window.transform, "close", buttonSize);
            UiFactory.CreateLabeledButton(UiIds.NewGame, window.transform, "restart", buttonSize);

            var purchase = UiFactory.CreateLabeledButton(UiIds.DisabledCounters, window.transform, "infiniteHints",
                buttonSize);
            var cost = UiFactory.Create(UiIds.Cost, purchase.transform);
            UiFactory.SetRect(UiFactory.Rect(cost), new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(1f, 0f), new Vector2(-16f, 12f), new Vector2(180f, 40f));
            var costText = UiFactory.Create(UiIds.CostText, cost.transform);
            UiFactory.Stretch(UiFactory.Rect(costText));
            UiFactory.AddText(costText, "", 28f, UiTheme.TextDark, TextAlignmentOptions.MidlineRight, false, true);

            UiFactory.CreateLabeledButton(UiIds.HardReset, window.transform, "resetData", buttonSize);
            BuildTopLineToggle(window.transform);
            options.SetActive(false);
        }

        private static void BuildTopLineToggle(Transform parent)
        {
            var go = UiFactory.Create(UiIds.ToggleTopLine, parent);
            UiFactory.SetRect(UiFactory.Rect(go), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(UiTheme.MenuButtonWidth, 80f));

            var background = UiFactory.Create(UiIds.ToggleBackground, go.transform);
            UiFactory.SetRect(UiFactory.Rect(background), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f), new Vector2(8f, 0f), new Vector2(50f, 50f));
            var bgImage = UiFactory.AddImage(background, Color.white);

            var check = UiFactory.Create(UiIds.Checkmark, background.transform);
            UiFactory.Stretch(UiFactory.Rect(check));
            var checkImage = UiFactory.AddImage(check, UiTheme.Icon, UiTheme.CheckBoxSprite);

            var label = UiFactory.Create("Text (TMP)", go.transform);
            UiFactory.Stretch(UiFactory.Rect(label), new Vector2(70f, 0f), new Vector2(-8f, 0f));
            UiFactory.AddText(label, "", 36f, UiTheme.TextDark, TextAlignmentOptions.MidlineLeft, false, true);
            UiFactory.BindLocalization(label, "showTopLine");

            var toggle = go.AddComponent<Toggle>();
            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;
        }

        private static void BuildStatistics(Transform canvas)
        {
            var stats = UiFactory.CreateStretchOverlay(UiIds.Statistics, canvas, UiTheme.OverlayDim);
            var content = UiFactory.Create(UiIds.Content, stats.transform);
            UiFactory.SetRect(UiFactory.Rect(content), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(-120f, -200f));
            AddVerticalMenuLayout(content, 16f);

            UiFactory.CreateLabeledButton(UiIds.Closed, content.transform, "close",
                new Vector2(UiTheme.MenuButtonWidth, 120f));

            var multiplier = UiFactory.Create(UiIds.Multiplier, content.transform);
            UiFactory.SetRect(UiFactory.Rect(multiplier), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 60f));
            UiFactory.AddText(multiplier, "", 40f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);

            var rating = UiFactory.Create(UiIds.PlayerRating, content.transform);
            UiFactory.SetRect(UiFactory.Rect(rating), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 50f));
            UiFactory.AddText(rating, "", 36f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);
            UiFactory.BindLocalization(rating, "playerRating");

            var leaderboard = UiFactory.Create(UiIds.Leaderboard, content.transform);
            UiFactory.SetRect(UiFactory.Rect(leaderboard), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1000f, 900f));
            var lbLayout = leaderboard.AddComponent<VerticalLayoutGroup>();
            lbLayout.childAlignment = TextAnchor.UpperCenter;
            lbLayout.spacing = 8f;
            lbLayout.childForceExpandWidth = true;
            lbLayout.childControlHeight = false;
            var le = leaderboard.AddComponent<LayoutElement>();
            le.minHeight = 700f;
            le.preferredHeight = 900f;
            stats.SetActive(false);
        }

        private static void BuildTutorialCaption(Transform gameSpace)
        {
            var caption = UiFactory.Create(UiIds.TutorialCaption, gameSpace);
            UiFactory.SetRect(UiFactory.Rect(caption), new Vector2(0f, 0f), new Vector2(1f, 0f),
                new Vector2(0.5f, 0f), Vector2.zero, new Vector2(0f, UiTheme.TutorialCaptionHeight));

            var text = UiFactory.Create(UiIds.TutorialCaptionText, caption.transform);
            UiFactory.Stretch(UiFactory.Rect(text), new Vector2(40f, 110f), new Vector2(-40f, -16f));
            UiFactory.AddText(text, "", 36f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);

            var continueBtn = UiFactory.Create(UiIds.TutorialContinue, caption.transform);
            UiFactory.SetRect(UiFactory.Rect(continueBtn), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f), new Vector2(0f, 16f), new Vector2(560f, 90f));
            UiFactory.AddImage(continueBtn, UiTheme.Button);
            UiFactory.AddButton(continueBtn);
            var label = UiFactory.Create("Text (TMP)", continueBtn.transform);
            UiFactory.Stretch(UiFactory.Rect(label));
            UiFactory.AddText(label, "", 40f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);
            UiFactory.BindLocalization(label, "continue");
            continueBtn.SetActive(false);

            caption.AddComponent<TutorialCaptionView>();
            caption.SetActive(false);
        }

        private static void BuildLoading(Transform canvas)
        {
            var loading = UiFactory.CreateStretchOverlay(UiIds.Loading, canvas, Color.white);
            loading.AddComponent<LoadingScreenManager>();
            var text = UiFactory.Create("Text (TMP)", loading.transform);
            UiFactory.Stretch(UiFactory.Rect(text));
            UiFactory.AddText(text, "...", 64f, UiTheme.TextDark, TextAlignmentOptions.Center);
        }

        private static void BuildConfirmation(Transform canvas)
        {
            var root = UiFactory.Create(UiIds.ConfirmationDialog, canvas);
            UiFactory.Stretch(UiFactory.Rect(root));
            UiFactory.AddImage(root, UiTheme.OverlayDim);

            var panel = UiFactory.Create(UiIds.Panel, root.transform);
            UiFactory.SetRect(UiFactory.Rect(panel), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800f, 450f));
            UiFactory.AddImage(panel, UiTheme.Panel);

            var message = UiFactory.Create(UiIds.Message, panel.transform);
            UiFactory.SetRect(UiFactory.Rect(message), new Vector2(0.05f, 0.4f), new Vector2(0.95f, 0.92f),
                new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            UiFactory.AddText(message, "", 36f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);
            message.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            BuildDialogButton(UiIds.Yes, UiIds.YesText, panel.transform, new Vector2(-180f, 50f));
            BuildDialogButton(UiIds.No, UiIds.NoText, panel.transform, new Vector2(180f, 50f));
            root.AddComponent<ConfirmationDialog>();
            root.SetActive(false);
        }

        private static void BuildDialogButton(string name, string textName, Transform parent, Vector2 position)
        {
            var go = UiFactory.Create(name, parent);
            UiFactory.SetRect(UiFactory.Rect(go), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f), position, new Vector2(280f, 90f));
            UiFactory.AddImage(go, UiTheme.Button);
            UiFactory.AddButton(go);
            var text = UiFactory.Create(textName, go.transform);
            UiFactory.Stretch(UiFactory.Rect(text), new Vector2(10f, 0f), new Vector2(-10f, 0f));
            UiFactory.AddText(text, "", 36f, UiTheme.TextDark, TextAlignmentOptions.Center, false, true);
        }

        private static void BuildLanguagePanel(Transform canvas)
        {
            var panelRoot = UiFactory.Create(UiIds.LanguagePanel, canvas);
            UiFactory.Stretch(UiFactory.Rect(panelRoot));

            var bgClose = UiFactory.Create(UiIds.BackgroundClose, panelRoot.transform);
            UiFactory.Stretch(UiFactory.Rect(bgClose));
            UiFactory.AddImage(bgClose, UiTheme.OverlayDim);
            UiFactory.AddButton(bgClose);

            var panel = UiFactory.Create(UiIds.Panel, panelRoot.transform);
            UiFactory.SetRect(UiFactory.Rect(panel), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520f, 900f));
            UiFactory.AddImage(panel, UiTheme.Panel);

            var close = UiFactory.Create(UiIds.Close, panel.transform);
            UiFactory.SetRect(UiFactory.Rect(close), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(1f, 1f), new Vector2(-12f, -12f), new Vector2(70f, 70f));
            UiFactory.AddImage(close, UiTheme.Button);
            UiFactory.AddButton(close);
            UiFactory.CreateText("X", close.transform, "×", 48f, UiTheme.TextDark, TextAlignmentOptions.Center);

            var list = UiFactory.Create(UiIds.Languages, panel.transform);
            UiFactory.SetRect(UiFactory.Rect(list), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(420f, 760f));
            var layout = list.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = 12f;
            layout.childForceExpandWidth = true;
            layout.childControlHeight = false;

            foreach (var code in UiIds.LanguageButtonNames)
            {
                var btn = UiFactory.Create(code, list.transform);
                UiFactory.SetRect(UiFactory.Rect(btn), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(400f, 80f));
                UiFactory.AddImage(btn, Color.white);
                UiFactory.AddButton(btn);
                var label = UiFactory.CreateText("Text (TMP)", btn.transform, UiTheme.GetNativeLanguageName(code),
                    40f, UiTheme.TextDark, TextAlignmentOptions.Center, true);
                if (code == "ZH") label.font = UiTheme.CjkFont;
            }

            panelRoot.SetActive(false);
        }

        private static void BuildVersion(Transform canvas)
        {
            var version = UiFactory.Create(UiIds.Version, canvas);
            UiFactory.SetRect(UiFactory.Rect(version), new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(1f, 0f), new Vector2(-8f, 8f), new Vector2(120f, 50f));
            UiFactory.AddText(version, UiTheme.VersionLabel, 24f, UiTheme.TextMuted, TextAlignmentOptions.MidlineRight);
        }

        private static void BuildGameplayServices()
        {
            var root = new GameObject(UiIds.RuntimeGameplay);
            CreateService<CellPool>(UiIds.CellPool, root.transform);
            CreateService<FloatingScorePool>(UiIds.FloatingScorePool, root.transform);
            CreateService<HeaderNumberDisplay>(UiIds.HeaderNumberDisplay, root.transform);
            CreateService<GridView>(UiIds.GridView, root.transform);
        }

        private static void CreateService<T>(string name, Transform parent) where T : Component
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<T>();
        }

        private static GameObject CreateCenteredWindow(string name, Transform overlay, float width, float spacing)
        {
            var window = UiFactory.Create(name, overlay);
            UiFactory.SetRect(UiFactory.Rect(window), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(width, 100f));
            AddVerticalMenuLayout(window, spacing);
            return window;
        }

        private static void AddVerticalMenuLayout(GameObject window, float spacing)
        {
            var layout = window.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = spacing;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            var fitter = window.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
}
