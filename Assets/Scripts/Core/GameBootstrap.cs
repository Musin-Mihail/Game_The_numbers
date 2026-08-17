using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Platform;
using Core.Shop;
using Core.UndoSystem;
using DataProviders;
using Gameplay;
using Interfaces;
using Localization;
using Model;
using UnityEngine;
using View.Grid;
using View.UI;
using View.UI.Builder;
using YG;

namespace Core
{
    /// <summary>
    /// Основной класс для инициализации игры. Отвечает за создание и регистрацию всех
    /// основных сервисов, моделей и контроллеров, а также за внедрение зависимостей.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        private GridView gridView;
        private HeaderNumberDisplay headerNumberDisplay;
        private ConfirmationDialog confirmationDialog;
        private LeaderboardUpdater leaderboardUpdater;
        private ShopManager shopManager;
        private GameManager gameManager;
        private LoadingScreenManager _loadingScreenManager;

        private const int MaxLoadAttempts = 3;
        private const float LoadAttemptDelay = 1.0f;

        private Action _requestNewGameAction;
        private GameController _gameController;
        private bool _isNewUser;
        private readonly List<IDisposable> _disposableServices = new();
        private LocalizationManager _localizationManager;

        private void BindCoreSystems()
        {
            // Ищем компоненты по типу. FindObjectsInactive.Include позволяет найти их даже на выключенных объектах UI!
            gridView = UnityEngine.Object.FindAnyObjectByType<GridView>(FindObjectsInactive.Include);
            headerNumberDisplay = UnityEngine.Object.FindAnyObjectByType<HeaderNumberDisplay>(FindObjectsInactive.Include);
            confirmationDialog = UnityEngine.Object.FindAnyObjectByType<ConfirmationDialog>(FindObjectsInactive.Include);
            leaderboardUpdater = UnityEngine.Object.FindAnyObjectByType<LeaderboardUpdater>(FindObjectsInactive.Include);
            shopManager = UnityEngine.Object.FindAnyObjectByType<ShopManager>(FindObjectsInactive.Include);
            gameManager = UnityEngine.Object.FindAnyObjectByType<GameManager>(FindObjectsInactive.Include);
            _loadingScreenManager = UnityEngine.Object.FindAnyObjectByType<LoadingScreenManager>(FindObjectsInactive.Include);

            if (gridView == null) Debug.LogWarning("[GameBootstrap] Компонент GridView не найден ни на одном объекте.");
            if (gameManager == null) Debug.LogWarning("[GameBootstrap] Компонент GameManager не найден ни на одном объекте.");
        }

        private void Awake()
        {
            ServiceProvider.Clear();

            _localizationManager = new LocalizationManager();
            ServiceProvider.Register(_localizationManager);

            var gridModel = new GridModel();
            var statisticsModel = new StatisticsModel();
            var actionCountersModel = new ActionCountersModel();
            var actionHistory = new ActionHistory();
            ServiceProvider.Register(gridModel);
            ServiceProvider.Register(statisticsModel);
            ServiceProvider.Register(actionCountersModel);
            ServiceProvider.Register(actionHistory);

            PlayingFieldUiBuilder.ReplaceSceneUi();
            BindCoreSystems();

            _loadingScreenManager?.Show();

            var yandexSaveLoadService = new YandexSaveLoadService(gridModel, statisticsModel, actionCountersModel);
            var yandexLeaderboardService = new YandexLeaderboardService(GameConstants.LeaderboardName);
            var yandexPlatformService = new YandexPlatformService();
            ServiceProvider.Register<ISaveLoadService>(yandexSaveLoadService);
            ServiceProvider.Register<ILeaderboardService>(yandexLeaderboardService);
            ServiceProvider.Register<IPlatformServices>(yandexPlatformService);
            _disposableServices.Add(yandexPlatformService);

            var tutorialHandler = new Core.Handlers.TutorialHandler(gridModel, yandexSaveLoadService, gameManager);
            ServiceProvider.Register(tutorialHandler);
            _disposableServices.Add(tutorialHandler);

            var gridDataProvider = new GridDataProvider(gridModel);
            var matchValidator = new MatchValidator(gridDataProvider);
            ServiceProvider.Register<IGridDataProvider>(gridDataProvider);
            ServiceProvider.Register(matchValidator);

            ServiceProvider.Register(gridView);
            ServiceProvider.Register(headerNumberDisplay);
            ServiceProvider.Register(confirmationDialog);

            var idleHintHandler = new Core.Handlers.IdleHintHandler(gridModel, matchValidator);
            ServiceProvider.Register(idleHintHandler);
            _disposableServices.Add(idleHintHandler);

            _gameController = new GameController(
                gridModel,
                matchValidator,
                actionHistory,
                actionCountersModel,
                statisticsModel,
                gameManager,
                gridView,
                yandexPlatformService
            );
            ServiceProvider.Register(_gameController);
            _disposableServices.Add(_gameController);

            InjectDependencies();
        }

        /// <summary>
        /// Внедряет зависимости в компоненты MonoBehaviour, которые не могут получить их через конструктор.
        /// </summary>
        private void InjectDependencies()
        {
            var saveLoadService = ServiceProvider.GetService<ISaveLoadService>();
            var leaderboardService = ServiceProvider.GetService<ILeaderboardService>();
            var actionCountersModel = ServiceProvider.GetService<ActionCountersModel>();

            if (gameManager != null) gameManager.Initialize(saveLoadService);
            else Debug.LogError("[GameBootstrap] GameManager не инициализирован!");

            if (leaderboardUpdater != null) leaderboardUpdater.Initialize(leaderboardService);
            else Debug.LogError("[GameBootstrap] LeaderboardUpdater не инициализирован!");

            if (shopManager != null) shopManager.Initialize(actionCountersModel);
            else Debug.LogError("[GameBootstrap] ShopManager не инициализирован!");

            if (gridView != null) gridView.Initialize(ServiceProvider.GetService<GridModel>(), headerNumberDisplay);
            else Debug.LogError("[GameBootstrap] GridView не инициализирован!");
        }

        /// <summary>
        /// Подписывается на события.
        /// </summary>
        private void OnEnable()
        {
            YG2.onGetSDKData += OnYandexSDKInitialized;
            YG2.onDefaultSaves += OnDefaultSavesReceived;
        }

        /// <summary>
        /// Отписывается от событий.
        /// </summary>
        private void OnDisable()
        {
            YG2.onGetSDKData -= OnYandexSDKInitialized;
            YG2.onDefaultSaves -= OnDefaultSavesReceived;
        }

        /// <summary>
        /// Вызывается, когда плагин определяет, что сохранений нет.
        /// </summary>
        private void OnDefaultSavesReceived()
        {
            _isNewUser = true;
        }

        /// <summary>
        /// Вызывается после успешной инициализации Yandex SDK. Запускает загрузку с повторными попытками.
        /// </summary>
        private void OnYandexSDKInitialized()
        {
            var langToLoad = YG2.lang;
            Debug.Log($"Язык определен из окружения SDK: '{langToLoad}'.");

            _localizationManager.SetInitialLanguage(langToLoad);
            GlobalEvents.OnYandexSDKInitialized?.Invoke();
            if (!gameManager) return;
            StartCoroutine(LoadGameWithRetries());
        }

        /// <summary>
        /// Корутина для загрузки игры с несколькими попытками.
        /// В случае неудачи - предлагает игроку выбор: повторить или начать новую игру.
        /// </summary>
        private IEnumerator LoadGameWithRetries()
        {
            if (_isNewUser)
            {
                Debug.Log("Плагин не нашел сохранений (onDefaultSaves). Запуск новой игры.");
                StartNewGameAndFinalize();
                yield break;
            }

            var saveLoadService = ServiceProvider.GetService<ISaveLoadService>();
            var attempts = 0;
            var loadSuccess = false;

            while (attempts < MaxLoadAttempts && !loadSuccess)
            {
                attempts++;
                Debug.Log($"Попытка загрузки данных #{attempts}");

                var loadFinished = false;
                saveLoadService.LoadGame(success =>
                {
                    loadSuccess = success;
                    loadFinished = true;
                });

                yield return new WaitUntil(() => loadFinished);

                if (loadSuccess || attempts >= MaxLoadAttempts) continue;
                Debug.LogWarning($"Загрузка не удалась. Повторная попытка через {LoadAttemptDelay} сек.");
                yield return new WaitForSeconds(LoadAttemptDelay);
            }

            if (loadSuccess)
            {
                if (!YG2.saves.seenMigrationIds.Contains(GameConstants.ScoreResetMigrationId))
                {
                    Debug.Log($"Выполняется миграция данных '{GameConstants.ScoreResetMigrationId}': сброс счета.");
                    YG2.saves.statistics.score = 0;
                    YG2.saves.record = 0;
                    YG2.saves.seenMigrationIds.Add(GameConstants.ScoreResetMigrationId);
                    var statisticsModel = ServiceProvider.GetService<StatisticsModel>();
                    statisticsModel.SetState(YG2.saves.statistics.score, YG2.saves.statistics.multiplier);
                    GlobalEvents.OnStatisticsChanged?.Invoke((statisticsModel.Score, statisticsModel.Multiplier));
                    saveLoadService.RequestSave();
                }

                Debug.Log("Данные успешно загружены. Отображение сохраненного состояния.");
                gridView.FullRedraw();
                FinalizeGameSetup();
            }
            else
            {
                Debug.LogError("Не удалось загрузить данные после нескольких попыток. Показ диалога ошибки.");
                ShowLoadErrorDialog();
            }
        }

        /// <summary>
        /// Показывает диалог с ошибкой загрузки и вариантами действий.
        /// </summary>
        private void ShowLoadErrorDialog()
        {
            var message = _localizationManager.Get("loadError");
            var retryText = _localizationManager.Get("retry");
            var newGameText = _localizationManager.Get("newGame");

            confirmationDialog.Show(
                message,
                retryText,
                newGameText,
                OnRetryLoad,
                OnStartNewGameWithWarning,
                new Vector2(0, 450)
            );
        }

        /// <summary>
        /// Обработчик для кнопки "Попробовать снова" в диалоге ошибки загрузки.
        /// </summary>
        private void OnRetryLoad()
        {
            Debug.Log("Игрок выбрал повторную попытку загрузки.");
            _loadingScreenManager?.Show();
            StartCoroutine(LoadGameWithRetries());
        }

        /// <summary>
        /// Обработчик для кнопки "Новая игра" в диалоге ошибки загрузки.
        /// </summary>
        private void OnStartNewGameWithWarning()
        {
            Debug.Log("Игрок выбрал начать новую игру после ошибки загрузки.");
            StartNewGameAndFinalize();
        }

        /// <summary>
        /// Запускает новую игру со сбросом прогресса и финализирует настройку.
        /// </summary>
        private void StartNewGameAndFinalize()
        {
            _gameController.StartNewGame(true);
            GlobalEvents.OnToggleTopLine?.Invoke(true);

            FinalizeGameSetup();
        }

        private void FinalizeGameSetup()
        {
            Debug.Log("Завершение настройки игры и активация UI.");
            SetupListeners();
            if (!YG2.saves.seenUpdateVersions.Contains(GameConstants.GameVersion))
            {
                GlobalEvents.OnNewUpdateAvailable?.Invoke();
            }

            _loadingScreenManager?.Hide();
        }

        private void SetupListeners()
        {
            if (confirmationDialog)
            {
                _requestNewGameAction = () =>
                {
                    var message = _localizationManager.Get("newGamePrompt");
                    var yes = _localizationManager.Get("yes");
                    var no = _localizationManager.Get("no");
                    confirmationDialog.Show(message, yes, no, StartNewGameFromButton, null, new Vector2(0, 450));
                };

                GlobalEvents.OnRequestNewGame += _requestNewGameAction;
                GlobalEvents.OnRequestRefillCounters += HandleRequestRefillCounters;
                GlobalEvents.OnRequestDisableCounters += HandleRequestDisableCounters;
            }
            else
            {
                GlobalEvents.OnRequestNewGame += StartNewGameFromButton;
            }

            GlobalEvents.OnRequestHardReset += HandleHardReset;
        }

        /// <summary>
        /// Обрабатывает запрос на полный сброс игры.
        /// Сбрасывает статус покупки и запускает новую игру со сбросом прогресса.
        /// </summary>
        private void HandleHardReset()
        {
            Debug.Log("Игрок запросил полный сброс. Сброс счетчиков и статистики.");
            var actionCountersModel = ServiceProvider.GetService<ActionCountersModel>();
            actionCountersModel?.ReEnableCounterLimits();
            YG2.saves.seenUpdateVersions.Clear();
            YG2.saves.seenMigrationIds.Clear();
            YG2.saves.record = 0;
            StartNewGameAndFinalize();
        }

        /// <summary>
        /// Обрабатывает запрос на отключение счетчиков, показывая диалог подтверждения.
        /// </summary>
        private void HandleRequestDisableCounters()
        {
            var message = _localizationManager.Get("buyInfiniteHintsPrompt");
            var yes = _localizationManager.Get("yes");
            var no = _localizationManager.Get("no");
            confirmationDialog.Show(message, yes, no, () => { GlobalEvents.OnDisableCountersConfirmed?.Invoke(); }, null, new Vector2(0, 350));
        }

        /// <summary>
        /// Обрабатывает запрос на пополнение счетчиков, показывая диалог подтверждения.
        /// </summary>
        private void HandleRequestRefillCounters()
        {
            var message = _localizationManager.Get("watchAdPrompt");
            var yes = _localizationManager.Get("yes");
            var no = _localizationManager.Get("no");
            confirmationDialog.Show(message, yes, no, () => { GlobalEvents.OnShowRewardedAdForRefill?.Invoke(); }, null, new Vector2(0, 370));
        }

        /// <summary>
        /// Вызывается при уничтожении объекта. Отписывается от всех событий и освобождает ресурсы.
        /// </summary>
        private void OnDestroy()
        {
            GlobalEvents.OnRequestHardReset -= HandleHardReset;
            if (confirmationDialog)
            {
                if (_requestNewGameAction != null)
                {
                    GlobalEvents.OnRequestNewGame -= _requestNewGameAction;
                }

                GlobalEvents.OnRequestRefillCounters -= HandleRequestRefillCounters;
                GlobalEvents.OnRequestDisableCounters -= HandleRequestDisableCounters;
            }
            else
            {
                GlobalEvents.OnRequestNewGame -= StartNewGameFromButton;
            }

            foreach (var service in _disposableServices)
            {
                service.Dispose();
            }

            _disposableServices.Clear();
            ServiceProvider.Clear();
        }

        /// <summary>
        /// Вызывается по нажатию кнопки "Новая игра".
        /// </summary>
        private void StartNewGameFromButton()
        {
            var statisticsModel = ServiceProvider.GetService<StatisticsModel>();
            statisticsModel.SetState(0, statisticsModel.Multiplier);
            GlobalEvents.OnStatisticsChanged?.Invoke((statisticsModel.Score, statisticsModel.Multiplier));
            _gameController.StartNewGame(false);
        }
    }
}