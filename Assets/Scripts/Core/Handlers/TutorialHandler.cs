using System;
using System.Collections.Generic;
using Core;
using Core.Events;
using Interfaces;
using Model;
using YG;

namespace Core.Handlers
{
    /// <summary>
    /// Управляет интерактивным туториалом для новых игроков.
    /// </summary>
    public class TutorialHandler : IDisposable
    {
        private readonly GameEvents _gameEvents;
        private readonly GridModel _gridModel;
        private readonly ISaveLoadService _saveLoadService;

        private int _currentStep = -1;
        private CellData _targetCell1;
        private CellData _targetCell2;

        public TutorialHandler(GameEvents gameEvents, GridModel gridModel, ISaveLoadService saveLoadService)
        {
            _gameEvents = gameEvents;
            _gridModel = gridModel;
            _saveLoadService = saveLoadService;

            _gameEvents.onTutorialStarted.AddListener(StartTutorial);
            _gameEvents.onMatchFound.AddListener(OnMatchFound);
        }

        private void StartTutorial()
        {
            _currentStep = 0;

            int[][] tutorialNumbers = new int[][]
            {
                new int[] { 4, 7, 7, 6, 1, 2, 3, 5, 8, 9 },
                new int[] { 9, 8, 5, 3, 2, 1, 6, 4, 2, 2 },
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1 }
            };

            var savedCells = new List<CellDataSerializable>();
            for (int i = 0; i < tutorialNumbers.Length; i++)
            {
                for (int j = 0; j < tutorialNumbers[i].Length; j++)
                {
                    savedCells.Add(new CellDataSerializable { line = i, column = j, number = tutorialNumbers[i][j], isActive = true });
                }
            }

            _gridModel.RestoreState(savedCells);
            _gameEvents.onNewGameStarted.Raise();

            SetStep(0);
        }

        private void SetStep(int step)
        {
            _currentStep = step;
            var cells = _gridModel.Cells;
            if (step == 0)
            {
                _targetCell1 = cells[0][1];
                _targetCell2 = cells[0][2];
            }
            else if (step == 1)
            {
                _targetCell1 = cells[0][0];
                _targetCell2 = cells[0][3];
            }
            else if (step == 2)
            {
                _targetCell1 = cells[0][9];
                _targetCell2 = cells[1][0];
            }

            var allowed = new List<Guid> { _targetCell1.Id, _targetCell2.Id };
            _gameEvents.onSetAllowedInputCells.Raise(allowed);
            _gameEvents.onHintFound.Raise((_targetCell1.Id, _targetCell2.Id));
        }

        private void OnMatchFound((Guid firstCellId, Guid secondCellId) data)
        {
            if (_currentStep < 0) return;

            if (_currentStep < 2)
            {
                SetStep(_currentStep + 1);
            }
            else
            {
                _currentStep = -1;
                YG2.saves.isTutorialCompleted = true;
                _saveLoadService.RequestSave();
                _gameEvents.onSetAllowedInputCells.Raise(null);
                _gameEvents.onTutorialCompleted.Raise();
            }
        }

        public void Dispose()
        {
            _gameEvents.onTutorialStarted.RemoveListener(StartTutorial);
            _gameEvents.onMatchFound.RemoveListener(OnMatchFound);
        }
    }
}
