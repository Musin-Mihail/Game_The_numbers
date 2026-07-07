using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.UndoSystem;
using Gameplay;
using Model;

namespace Core.Handlers
{
    /// <summary>
    /// Обрабатывает логику совпадения пар: проверяет валидность, обновляет модели,
    /// начисляет очки и удаляет пустые линии.
    /// </summary>
    public class MatchHandler : IDisposable
    {
        private readonly GridModel _gridModel;
        private readonly MatchValidator _matchValidator;
        private readonly StatisticsModel _statisticsModel;
        private readonly ActionHistory _actionHistory;
        private readonly GameManager _gameManager;

        /// <summary>
        /// Инициализирует обработчик совпадений с необходимыми зависимостями.
        /// </summary>
        public MatchHandler(
            GridModel gridModel,
            MatchValidator matchValidator,
            StatisticsModel statisticsModel,
            ActionHistory actionHistory,
            GameManager gameManager)
        {
            _gridModel = gridModel;
            _matchValidator = matchValidator;
            _statisticsModel = statisticsModel;
            _actionHistory = actionHistory;
            _gameManager = gameManager;

            GlobalEvents.OnAttemptMatch += AttemptMatch;
        }

        /// <summary>
        /// Отписывается от событий.
        /// </summary>
        public void Dispose()
        {
            GlobalEvents.OnAttemptMatch -= AttemptMatch;
        }

        /// <summary>
        /// Обрабатывает попытку составить пару из двух ячеек.
        /// </summary>
        private void AttemptMatch((Guid firstCellId, Guid secondCellId) data)
        {
            var firstData = _gridModel.GetCellDataById(data.firstCellId);
            var secondData = _gridModel.GetCellDataById(data.secondCellId);

            if (firstData != null && secondData != null && _matchValidator.IsAValidMatch(firstData, secondData))
            {
                ProcessValidMatch(firstData, secondData);
            }
            else
            {
                GlobalEvents.OnInvalidMatch?.Invoke();
            }
        }

        /// <summary>
        /// Обрабатывает допустимое совпадение.
        /// </summary>
        private void ProcessValidMatch(CellData data1, CellData data2)
        {
            var removedLinesInfo = new List<Tuple<int, List<CellData>>>();
            var lineScores = new Dictionary<int, int>();
            var scoreBeforeAction = _statisticsModel.Score;
            var multiplierBeforeAction = _statisticsModel.Multiplier;

            GlobalEvents.OnMatchFound?.Invoke((data1.Id, data2.Id));

            _gridModel.SetCellActiveState(data1, false);
            _gridModel.SetCellActiveState(data2, false);

            var pairScore = 1 * _statisticsModel.Multiplier;
            _statisticsModel.AddScore(pairScore);
            GlobalEvents.OnPairScoreAdded?.Invoke((data1.Id, data2.Id, pairScore));

            CheckAndRemoveEmptyLines(data1.Line, data2.Line, removedLinesInfo, lineScores);

            if (_gridModel.GetAllActiveCellData().Count == 0)
            {
                _statisticsModel.IncrementMultiplier();
                GlobalEvents.OnBoardCleared?.Invoke();
            }

            var action = new MatchAction(data1.Id, data2.Id, removedLinesInfo, scoreBeforeAction, multiplierBeforeAction, pairScore, lineScores, _gridModel, _statisticsModel);
            _actionHistory.Record(action);

            GlobalEvents.OnStatisticsChanged?.Invoke((_statisticsModel.Score, _statisticsModel.Multiplier));
            _gameManager?.RequestSave();
        }

        /// <summary>
        /// Проверяет и удаляет пустые линии.
        /// </summary>
        private void CheckAndRemoveEmptyLines(int line1, int line2, List<Tuple<int, List<CellData>>> removedLinesInfo, Dictionary<int, int> lineScores)
        {
            var linesToRemove = new HashSet<int>();
            if (_gridModel.IsLineEmpty(line1)) linesToRemove.Add(line1);
            if (line1 != line2 && _gridModel.IsLineEmpty(line2)) linesToRemove.Add(line2);

            if (linesToRemove.Count <= 0) return;

            foreach (var lineIndex in linesToRemove.OrderByDescending(i => i))
            {
                var lineData = new List<CellData>(_gridModel.Cells[lineIndex]);
                removedLinesInfo.Add(new Tuple<int, List<CellData>>(lineIndex, lineData));
                _gridModel.RemoveLine(lineIndex);

                var scoreForLine = 10 * _statisticsModel.Multiplier;
                _statisticsModel.AddScore(scoreForLine);
                lineScores[lineIndex] = scoreForLine;
                GlobalEvents.OnLineScoreAdded?.Invoke((lineIndex, scoreForLine));
            }

            GlobalEvents.OnLinesRemoved?.Invoke();
        }
    }
}