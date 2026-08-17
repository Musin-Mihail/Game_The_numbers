using System;
using Core.Events;
using Gameplay;
using Model;
using View.Grid;
using YandexGames;

namespace Core.Handlers
{
    /// <summary>
    /// Обрабатывает логику, связанную с подсказками: поиск допустимой пары и отображение.
    /// </summary>
    public class HintHandler : IDisposable
    {
        private readonly GridModel _gridModel;
        private readonly MatchValidator _matchValidator;
        private readonly ActionCountersModel _actionCountersModel;
        private readonly GridView _gridView;
        private readonly GameManager _gameManager;

        /// <summary>
        /// Инициализирует обработчик подсказок с необходимыми зависимостями.
        /// </summary>
        public HintHandler(
            GridModel gridModel,
            MatchValidator matchValidator,
            ActionCountersModel actionCountersModel,
            GridView gridView,
            GameManager gameManager)
        {
            _gridModel = gridModel;
            _matchValidator = matchValidator;
            _actionCountersModel = actionCountersModel;
            _gridView = gridView;
            _gameManager = gameManager;

            GlobalEvents.OnRequestHint += FindAndShowHint;
        }

        /// <summary>
        /// Отписывается от событий.
        /// </summary>
        public void Dispose()
        {
            GlobalEvents.OnRequestHint -= FindAndShowHint;
        }

        /// <summary>
        /// Ищет и отображает подсказку (первую найденную допустимую пару).
        /// </summary>
        private void FindAndShowHint()
        {
            if (!YandexGamesSdk.Saves.isTutorialCompleted) return;

            if (_gridView.HasActiveHints)
            {
                return;
            }

            if (!_actionCountersModel.IsHintAvailable())
            {
                GlobalEvents.OnRequestRefillCounters?.Invoke();
                return;
            }

            var activeCells = _gridModel.GetAllActiveCellData();
            if (activeCells.Count < 2)
            {
                GlobalEvents.OnNoHintFound?.Invoke();
                return;
            }

            for (var i = 0; i < activeCells.Count; i++)
            {
                for (var j = i + 1; j < activeCells.Count; j++)
                {
                    var cell1 = activeCells[i];
                    var cell2 = activeCells[j];

                    if (!_matchValidator.IsAValidMatch(cell1, cell2)) continue;

                    ShowHintAndDecrementCounter(cell1, cell2);
                    return;
                }
            }

            GlobalEvents.OnNoHintFound?.Invoke();
        }

        /// <summary>
        /// Отображает подсказку и уменьшает счетчик.
        /// </summary>
        private void ShowHintAndDecrementCounter(CellData cell1, CellData cell2)
        {
            if (!_actionCountersModel.AreCountersDisabled)
            {
                _actionCountersModel.DecrementHint();
            }

            GlobalEvents.OnHintFound?.Invoke((cell1.Id, cell2.Id));
            _gameManager?.RequestSave();
        }
    }
}