using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Interfaces;
using Model;
using UnityEngine;
using YandexGames;

namespace Core.Handlers
{
    /// <summary>
    /// Интерактивное обучение: подсветка пар и текст правил под сеткой.
    /// </summary>
    public class TutorialHandler : IDisposable
    {
        private static readonly TutorialStep[] Steps =
        {
            new(0, 2, 0, 3, "tutorialSameNumber"),
            new(0, 1, 0, 4, "tutorialSumTen"),
            new(0, 5, 1, 5, "tutorialVertical"),
            new(0, 9, 1, 0, "tutorialLineWrap"),
            new(0, 0, 2, 9, "tutorialFirstLast"),
            TutorialStep.CaptionOnly("tutorialScoring")
        };

        private readonly GridModel _gridModel;
        private readonly ISaveLoadService _saveLoadService;
        private readonly GameManager _gameManager;

        private int _currentStep = -1;
        private Coroutine _nextStepCoroutine;

        public TutorialHandler(GridModel gridModel, ISaveLoadService saveLoadService, GameManager gameManager)
        {
            _gridModel = gridModel;
            _saveLoadService = saveLoadService;
            _gameManager = gameManager;

            GlobalEvents.OnTutorialStarted += StartTutorial;
            GlobalEvents.OnMatchFound += OnMatchFound;
            GlobalEvents.OnTutorialContinue += OnContinue;
        }

        private void StartTutorial()
        {
            StopPendingStep();
            _currentStep = 0;

            int[][] tutorialNumbers =
            {
                new[] { 1, 4, 7, 7, 6, 3, 5, 8, 2, 9 },
                new[] { 9, 8, 5, 2, 4, 7, 6, 1, 2, 2 },
                new[] { 2, 3, 4, 5, 6, 8, 9, 2, 8, 1 }
            };

            var savedCells = new List<CellDataSerializable>();
            for (var i = 0; i < tutorialNumbers.Length; i++)
            {
                for (var j = 0; j < tutorialNumbers[i].Length; j++)
                {
                    savedCells.Add(new CellDataSerializable
                    {
                        line = i,
                        column = j,
                        number = tutorialNumbers[i][j],
                        isActive = true
                    });
                }
            }

            _gridModel.RestoreState(savedCells);
            GlobalEvents.OnHideMenu?.Invoke();
            GlobalEvents.OnHideOptions?.Invoke();
            GlobalEvents.OnNewGameStarted?.Invoke();

            SetStep(0);
        }

        private void SetStep(int step)
        {
            _currentStep = step;
            var tutorialStep = Steps[step];
            GlobalEvents.OnTutorialCaptionChanged?.Invoke((tutorialStep.CaptionKey, tutorialStep.ShowContinue));

            if (!tutorialStep.RequiresMatch)
            {
                GlobalEvents.OnSetAllowedInputCells?.Invoke(new List<Guid>());
                return;
            }

            var cells = _gridModel.Cells;
            var cell1 = cells[tutorialStep.Line1][tutorialStep.Column1];
            var cell2 = cells[tutorialStep.Line2][tutorialStep.Column2];
            GlobalEvents.OnSetAllowedInputCells?.Invoke(new List<Guid> { cell1.Id, cell2.Id });
            GlobalEvents.OnHintFound?.Invoke((cell1.Id, cell2.Id));
        }

        private void OnMatchFound((Guid firstCellId, Guid secondCellId) data)
        {
            if (_currentStep < 0) return;
            if (!Steps[_currentStep].RequiresMatch) return;

            StopPendingStep();
            _nextStepCoroutine = _gameManager.StartCoroutine(NextStepRoutine());
        }

        private void OnContinue()
        {
            if (_currentStep < 0) return;
            if (Steps[_currentStep].RequiresMatch) return;
            CompleteTutorial();
        }

        private IEnumerator NextStepRoutine()
        {
            yield return new WaitForSeconds(0.6f);
            _nextStepCoroutine = null;
            var next = _currentStep + 1;
            if (next < Steps.Length)
            {
                SetStep(next);
            }
            else
            {
                CompleteTutorial();
            }
        }

        private void CompleteTutorial()
        {
            StopPendingStep();
            _currentStep = -1;
            YandexGamesSdk.Saves.isTutorialCompleted = true;
            _saveLoadService.RequestSave();
            GlobalEvents.OnSetAllowedInputCells?.Invoke(null);
            GlobalEvents.OnTutorialCaptionChanged?.Invoke((null, false));
            GlobalEvents.OnTutorialCompleted?.Invoke();
        }

        private void StopPendingStep()
        {
            if (_nextStepCoroutine == null || _gameManager == null) return;
            _gameManager.StopCoroutine(_nextStepCoroutine);
            _nextStepCoroutine = null;
        }

        public void Dispose()
        {
            StopPendingStep();
            GlobalEvents.OnTutorialStarted -= StartTutorial;
            GlobalEvents.OnMatchFound -= OnMatchFound;
            GlobalEvents.OnTutorialContinue -= OnContinue;
        }

        private readonly struct TutorialStep
        {
            public readonly int Line1;
            public readonly int Column1;
            public readonly int Line2;
            public readonly int Column2;
            public readonly string CaptionKey;
            public readonly bool RequiresMatch;
            public readonly bool ShowContinue;

            public TutorialStep(int line1, int column1, int line2, int column2, string captionKey)
            {
                Line1 = line1;
                Column1 = column1;
                Line2 = line2;
                Column2 = column2;
                CaptionKey = captionKey;
                RequiresMatch = true;
                ShowContinue = false;
            }

            private TutorialStep(string captionKey)
            {
                Line1 = -1;
                Column1 = -1;
                Line2 = -1;
                Column2 = -1;
                CaptionKey = captionKey;
                RequiresMatch = false;
                ShowContinue = true;
            }

            public static TutorialStep CaptionOnly(string captionKey) => new(captionKey);
        }
    }
}
