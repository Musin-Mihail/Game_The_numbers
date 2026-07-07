using System;
using Core.Events;
using Gameplay;
using Model;

namespace Core.Handlers
{
    /// <summary>
    /// Управляет системой ненавязчивых подсказок (Idle Hint).
    /// </summary>
    public class IdleHintHandler : IDisposable
    {
        private readonly GameEvents _gameEvents;
        private readonly GridModel _gridModel;
        private readonly MatchValidator _matchValidator;

        private float _idleTimer;
        private const float IdleThreshold = 15f;

        public IdleHintHandler(GameEvents gameEvents, GridModel gridModel, MatchValidator matchValidator)
        {
            _gameEvents = gameEvents;
            _gridModel = gridModel;
            _matchValidator = matchValidator;

            _gameEvents.onAttemptMatch.AddListener(ResetTimerAttempt);
            _gameEvents.onGridCleared.AddListener(ResetTimerVoid);
            _gameEvents.onCellAdded.AddListener(ResetTimerCellData);
        }

        public void Tick(float deltaTime)
        {
            if (!YG.YG2.saves.isTutorialCompleted) return;
            _idleTimer += deltaTime;
            if (_idleTimer >= IdleThreshold)
            {
                _idleTimer = 0f;
                ShowIdleHint();
            }
        }

        private void ResetTimerAttempt((Guid, Guid) data) => _idleTimer = 0f;
        private void ResetTimerVoid() => _idleTimer = 0f;
        private void ResetTimerCellData((CellData, bool) data) => _idleTimer = 0f;

        private void ShowIdleHint()
        {
            var activeCells = _gridModel.GetAllActiveCellData();
            if (activeCells.Count < 2) return;

            for (var i = 0; i < activeCells.Count; i++)
            {
                for (var j = i + 1; j < activeCells.Count; j++)
                {
                    var cell1 = activeCells[i];
                    var cell2 = activeCells[j];

                    if (_matchValidator.IsAValidMatch(cell1, cell2))
                    {
                        _gameEvents.onIdleHintFound.Raise((cell1.Id, cell2.Id));
                        return;
                    }
                }
            }
        }

        public void Dispose()
        {
            _gameEvents.onAttemptMatch.RemoveListener(ResetTimerAttempt);
            _gameEvents.onGridCleared.RemoveListener(ResetTimerVoid);
            _gameEvents.onCellAdded.RemoveListener(ResetTimerCellData);
        }
    }
}
