using System;
using Core.Events;
using Core.UndoSystem;
using Localization;
using Model;
using UnityEngine;
using View.UI;

namespace Core.Handlers
{
    /// <summary>
    /// Обрабатывает действия игрока, такие как отмена хода и добавление новых чисел.
    /// </summary>
    public class PlayerActionHandler : IDisposable
    {
        private readonly ActionCountersModel _actionCountersModel;
        private readonly ActionHistory _actionHistory;
        private readonly GridModel _gridModel;
        private readonly StatisticsModel _statisticsModel;
        private readonly GameManager _gameManager;
        private readonly ConfirmationDialog _confirmationDialog;
        private readonly LocalizationManager _localizationManager;
        private readonly GameController _gameController;
        private const int LineLimit = 1000;

        /// <summary>
        /// Инициализирует обработчик действий игрока с необходимыми зависимостями.
        /// </summary>
        public PlayerActionHandler(
            ActionCountersModel actionCountersModel,
            ActionHistory actionHistory,
            GridModel gridModel,
            StatisticsModel statisticsModel,
            GameManager gameManager,
            GameController gameController)
        {
            _actionCountersModel = actionCountersModel;
            _actionHistory = actionHistory;
            _gridModel = gridModel;
            _statisticsModel = statisticsModel;
            _gameManager = gameManager;
            _gameController = gameController;

            _confirmationDialog = ServiceProvider.GetService<ConfirmationDialog>();
            _localizationManager = ServiceProvider.GetService<LocalizationManager>();

            GlobalEvents.OnAddExistingNumbers += AddExistingNumbersAsNewLines;
            GlobalEvents.OnUndoLastAction += UndoLastAction;
        }

        /// <summary>
        /// Отписывается от событий.
        /// </summary>
        public void Dispose()
        {
            GlobalEvents.OnAddExistingNumbers -= AddExistingNumbersAsNewLines;
            GlobalEvents.OnUndoLastAction -= UndoLastAction;
        }

        /// <summary>
        /// Добавляет существующие на поле числа в виде новых линий.
        /// </summary>
        private void AddExistingNumbersAsNewLines()
        {
            if (!YG.YG2.saves.isTutorialCompleted) return;

            if (_gridModel.Cells.Count * 2 > LineLimit)
            {
                var message = _localizationManager.Get("lineLimitReached");
                var newGameText = _localizationManager.Get("newGame");
                var continueText = _localizationManager.Get("continue");
                _confirmationDialog.Show(
                    message,
                    newGameText,
                    continueText,
                    () => _gameController.StartNewGame(false),
                    null,
                    new Vector2(0, 350)
                );
            }
            else
            {
                _gridModel.AppendActiveNumbersToGrid();
                _actionHistory.Clear();
                _gameManager?.RequestSave();
            }
        }

        /// <summary>
        /// Отменяет последнее действие игрока.
        /// </summary>
        private void UndoLastAction()
        {
            if (!YG.YG2.saves.isTutorialCompleted) return;

            if (!_actionCountersModel.IsUndoAvailable())
            {
                GlobalEvents.OnRequestRefillCounters?.Invoke();
                return;
            }

            if (!_actionHistory.CanUndo()) return;
            _actionHistory.Undo();
            if (!_actionCountersModel.AreCountersDisabled)
            {
                _actionCountersModel.DecrementUndo();
            }

            GlobalEvents.OnStatisticsChanged?.Invoke((_statisticsModel.Score, _statisticsModel.Multiplier));
            _gameManager?.RequestSave();
        }
    }
}