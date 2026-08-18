using System;
using System.Collections.Generic;
using Model;

namespace Core.Events
{
    public static class GlobalEvents
    {
        public static Action OnYandexSDKInitialized;
        public static Action OnRequestNewGame;
        public static Action OnNewGameStarted;
        public static Action OnRequestHardReset;

        public static Action<(CellData cell, bool animate)> OnCellAdded;
        public static Action<CellData> OnCellUpdated;
        public static Action<Guid> OnCellRemoved;
        public static Action OnGridCleared;
        public static Action OnLinesRemoved;

        public static Action OnUndoLastAction;
        public static Action OnAddExistingNumbers;
        public static Action OnRequestHint;

        public static Action<(Guid firstCellId, Guid secondCellId)> OnAttemptMatch;
        public static Action<(Guid firstCellId, Guid secondCellId)> OnMatchFound;
        public static Action OnInvalidMatch;

        public static Action<(Guid firstId, Guid secondId)> OnHintFound;
        public static Action<(Guid id1, Guid id2)> OnIdleHintFound;
        public static Action OnNoHintFound;

        public static Action OnTutorialStarted;
        public static Action OnTutorialCompleted;
        public static Action OnTutorialContinue;
        public static Action<(string localizationKey, bool showContinue)> OnTutorialCaptionChanged;
        public static Action<List<Guid>> OnSetAllowedInputCells;

        public static Action OnRequestRefillCounters;
        public static Action OnRequestDisableCounters;
        public static Action OnDisableCountersConfirmed;
        public static Action<(int undo, int hint)> OnCountersChanged;

        public static Action OnShowRewardedAdForRefill;

        public static Action<(Guid cell1, Guid cell2, int score)> OnPairScoreAdded;
        public static Action<(int lineIndex, int score)> OnLineScoreAdded;
        public static Action<(Guid cell1, Guid cell2, int score)> OnPairScoreUndone;
        public static Action<(int lineIndex, int score)> OnLineScoreUndone;
        public static Action<(long score, int multiplier)> OnStatisticsChanged;
        public static Action OnBoardCleared;

        public static Action<bool> OnToggleTopLine;
        public static Action OnShowMenu;
        public static Action OnHideMenu;
        public static Action OnShowStatistics;
        public static Action OnHideStatistics;
        public static Action OnShowOptions;
        public static Action OnHideOptions;
        public static Action<string> OnSetLanguage;

        public static Action OnNewUpdateAvailable;
        public static Action OnUpdateSeen;
        public static Action OnRequestMarkUpdateSeen;
    }
}
