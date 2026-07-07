using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Model;
using UnityEngine;
using UnityEngine.UI;
using View.UI;

namespace View.Grid
{
    /// <summary>
    /// Основной компонент-оркестратор для представления игровой сетки.
    /// Реализует виртуализацию UI, отображая только видимые ячейки.
    /// </summary>
    public class GridView : MonoBehaviour
    {
        private CellPool _cellPool;
        private FloatingScorePool _floatingScorePool;
        private RectTransform _contentContainer;
        private ScrollRect _scrollRect;
        private RectTransform _scrollviewContainer;

        [Header("Настройки")]
        [SerializeField] private Color positiveScoreColor = Color.green;
        [SerializeField] private Color negativeScoreColor = Color.red;
        [Tooltip("Количество рядов ячеек, которые будут созданы за пределами видимой области для плавной прокрутки.")]
        [SerializeField] private int lineBuffer = 2;

        private readonly Dictionary<Guid, Cell> _cellViewInstances = new();

        private GridModel _gridModel;
        private HeaderNumberDisplay _headerNumberDisplay;

        private GridInputHandler _inputHandler;
        private GridVisuals _visuals;
        private GridLayoutManager _layoutManager;

        /// <summary>
        /// Указывает, есть ли в данный момент активные (подсвеченные) подсказки.
        /// </summary>
        public bool HasActiveHints => _visuals?.HasActiveHints ?? false;

        /// <summary>
        /// Инициализация зависимостей, полученных из GameBootstrap.
        /// </summary>
        public void Initialize(GridModel gridModel, HeaderNumberDisplay headerNumberDisplay)
        {
            _gridModel = gridModel;
            _headerNumberDisplay = headerNumberDisplay;
        }

        private void Awake()
        {
            BindUI();
            _visuals = new GridVisuals(_cellViewInstances, _floatingScorePool, positiveScoreColor, negativeScoreColor);
            _inputHandler = new GridInputHandler(_visuals);
            _layoutManager = new GridLayoutManager(_contentContainer, _scrollRect, _scrollviewContainer, _headerNumberDisplay, _gridModel);

            _layoutManager.Initialize();
        }

        private void BindUI()
        {
            _cellPool = UnityEngine.Object.FindFirstObjectByType<CellPool>(FindObjectsInactive.Include);
            _floatingScorePool = UnityEngine.Object.FindFirstObjectByType<FloatingScorePool>(FindObjectsInactive.Include);
            
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in allTransforms)
            {
                if (t.name == "ScrollView" && t.GetComponent<ScrollRect>() != null)
                {
                    _scrollRect = t.GetComponent<ScrollRect>();
                    break;
                }
            }

            if (_scrollRect != null)
            {
                // Назначаем сам ScrollView в качестве контейнера, чтобы отступ (Padding) 
                // сдвигал всё игровое поле вниз и освобождал место для верхней строки
                _scrollviewContainer = _scrollRect.GetComponent<RectTransform>();
                _contentContainer = _scrollRect.content;
            }
            else
            {
                Debug.LogError("[GridView] Компонент ScrollRect с именем 'ScrollView' не найден!");
            }
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void OnDestroy()
        {
            _layoutManager.Dispose();
        }

        private void SubscribeToEvents()
        {
            GlobalEvents.OnNewGameStarted += FullRedraw;
            GlobalEvents.OnCellAdded += HandleGridChanged;
            GlobalEvents.OnCellUpdated += HandleCellUpdated;
            GlobalEvents.OnCellRemoved += HandleGridChanged;
            GlobalEvents.OnGridCleared += HandleGridCleared;
            GlobalEvents.OnMatchFound += HandleMatchFound;
            GlobalEvents.OnInvalidMatch += HandleInvalidMatch;
            GlobalEvents.OnToggleTopLine += HandleToggleTopLine;
            GlobalEvents.OnHintFound += HandleHintFound;
            GlobalEvents.OnIdleHintFound += HandleIdleHintFound;
            GlobalEvents.OnPairScoreAdded += HandlePairScoreAdded;
            GlobalEvents.OnLineScoreAdded += HandleLineScoreAdded;
            GlobalEvents.OnPairScoreUndone += HandlePairScoreUndone;
            GlobalEvents.OnLineScoreUndone += HandleLineScoreUndone;
            GlobalEvents.OnBoardCleared += HandleBoardCleared;
            GlobalEvents.OnLinesRemoved += HandleGridChanged;
            GlobalEvents.OnSetAllowedInputCells += HandleSetAllowedCells;

            if (_scrollRect != null) _scrollRect.onValueChanged.AddListener(OnScrollPositionChanged);
        }

        private void UnsubscribeFromEvents()
        {
            GlobalEvents.OnNewGameStarted -= FullRedraw;
            GlobalEvents.OnCellAdded -= HandleGridChanged;
            GlobalEvents.OnCellUpdated -= HandleCellUpdated;
            GlobalEvents.OnCellRemoved -= HandleGridChanged;
            GlobalEvents.OnGridCleared -= HandleGridCleared;
            GlobalEvents.OnMatchFound -= HandleMatchFound;
            GlobalEvents.OnInvalidMatch -= HandleInvalidMatch;
            GlobalEvents.OnToggleTopLine -= HandleToggleTopLine;
            GlobalEvents.OnHintFound -= HandleHintFound;
            GlobalEvents.OnIdleHintFound -= HandleIdleHintFound;
            GlobalEvents.OnPairScoreAdded -= HandlePairScoreAdded;
            GlobalEvents.OnLineScoreAdded -= HandleLineScoreAdded;
            GlobalEvents.OnPairScoreUndone -= HandlePairScoreUndone;
            GlobalEvents.OnLineScoreUndone -= HandleLineScoreUndone;
            GlobalEvents.OnBoardCleared -= HandleBoardCleared;
            GlobalEvents.OnLinesRemoved -= HandleGridChanged;
            GlobalEvents.OnSetAllowedInputCells -= HandleSetAllowedCells;

            if (_scrollRect != null) _scrollRect.onValueChanged.RemoveListener(OnScrollPositionChanged);
        }

        private void HandleSetAllowedCells(List<Guid> allowed)
        {
            _inputHandler.SetAllowedCells(allowed);
        }

        private void OnScrollPositionChanged(Vector2 pos)
        {
            UpdateVisibleCells();
            _layoutManager.RefreshTopLine();
        }

        /// <summary>
        /// Основной метод виртуализации. Создает/удаляет вьюшки ячеек на основе их видимости.
        /// </summary>
        private void UpdateVisibleCells()
        {
            if (_gridModel == null || !_scrollRect) return;
            // 1. Рассчитать диапазон видимых линий
            var (firstVisibleLine, lastVisibleLine) = _layoutManager.GetVisibleLineRange(lineBuffer);
            // 2. Определить, какие ячейки должны быть видимы
            var requiredCells = new HashSet<Guid>();
            for (var i = firstVisibleLine; i <= lastVisibleLine; i++)
            {
                if (i < 0 || i >= _gridModel.Cells.Count) continue;
                foreach (var cellData in _gridModel.Cells[i])
                {
                    requiredCells.Add(cellData.Id);
                }
            }

            // 3. Удалить ячейки, которые больше не видны
            var cellsToRemove = _cellViewInstances.Keys.Where(cellId => !requiredCells.Contains(cellId)).ToList();
            foreach (var cellId in cellsToRemove)
            {
                if (!_cellViewInstances.TryGetValue(cellId, out var cellView)) continue;
                // Перед удалением сбрасываем состояние, чтобы не было "глюков" при переиспользовании
                _visuals.ClearCellVisuals(cellView);
                _cellPool.ReturnCell(cellView);
                _cellViewInstances.Remove(cellId);
            }

            // 4. Добавить ячейки, которые стали видимыми
            for (var i = firstVisibleLine; i <= lastVisibleLine; i++)
            {
                if (i < 0 || i >= _gridModel.Cells.Count) continue;
                foreach (var cellData in _gridModel.Cells[i])
                {
                    if (_cellViewInstances.ContainsKey(cellData.Id)) continue;
                    var newCellView = _cellPool.GetCell();
                    newCellView.UpdateFromData(cellData);
                    newCellView.OnClickedCallback = _inputHandler.HandleCellClicked;
                    // Применяем сохраненные состояния (выделение, подсказка)
                    newCellView.SetSelected(_inputHandler.IsCellSelected(cellData.Id));
                    newCellView.SetHighlight(_visuals.IsCellHinted(cellData.Id));
                    _cellViewInstances[cellData.Id] = newCellView;
                    _layoutManager.UpdateCellPosition(cellData, newCellView, false); // Без анимации при прокрутке
                }
            }
        }

        public void ResetSelectionAndHints()
        {
            _inputHandler.ResetSelection();
            _visuals.ClearHintVisuals();
        }

        /// <summary>
        /// Полностью перерисовывает сетку на основе текущего состояния GridModel.
        /// </summary>
        public void FullRedraw()
        {
            HandleGridCleared();
            _layoutManager.UpdateContentSize();
            UpdateVisibleCells();
            _layoutManager.RefreshTopLine();
        }

        private void HandleGridChanged((CellData cell, bool animate) payload) => HandleGridChanged();
        private void HandleGridChanged(Guid payload) => HandleGridChanged();

        private void HandleGridChanged()
        {
            _layoutManager.UpdateContentSize();
            UpdateVisibleCells();
        }

        private void HandleCellUpdated(CellData data)
        {
            if (!_cellViewInstances.TryGetValue(data.Id, out var cellView)) return;
            cellView.UpdateFromData(data);
            _layoutManager.UpdateCellPosition(data, cellView, true);
        }

        private void HandleGridCleared()
        {
            _inputHandler.ResetSelection();
            _visuals.ClearHintVisuals();

            foreach (var cell in _cellViewInstances.Values)
            {
                _cellPool.ReturnCell(cell);
            }

            _cellViewInstances.Clear();
            _layoutManager.UpdateContentSize();
        }

        private void HandleBoardCleared()
        {
            if (_scrollRect != null) _visuals.ShowBoardClearedMessage(_scrollRect.viewport);
        }

        private void HandleHintFound((Guid firstId, Guid secondId) data)
        {
            _visuals.ShowHint(data);
        }

        private void HandleIdleHintFound((Guid id1, Guid id2) data)
        {
            if (_cellViewInstances.TryGetValue(data.id1, out var cell1))
            {
                cell1.Animator.Wiggle(cell1.TargetRectTransform);
            }
            if (_cellViewInstances.TryGetValue(data.id2, out var cell2))
            {
                cell2.Animator.Wiggle(cell2.TargetRectTransform);
            }
        }

        private void HandleMatchFound((Guid firstCellId, Guid secondCellId) data)
        {
            _visuals.ClearHintVisuals();
        }

        private void HandleInvalidMatch()
        {
            _visuals.ClearHintVisuals();
        }

        private void HandleToggleTopLine(bool isActive)
        {
            _layoutManager.SetTopPaddingActive(isActive);
        }

        private void HandlePairScoreAdded((Guid cell1, Guid cell2, int score) data)
        {
            var pos1 = _layoutManager.GetCellPosition(data.cell1);
            var pos2 = _layoutManager.GetCellPosition(data.cell2);
            if (pos1.HasValue && pos2.HasValue)
            {
                _visuals.ShowFloatingScoreForPair(pos1.Value, pos2.Value, data.score, true);
            }
        }

        private void HandlePairScoreUndone((Guid cell1, Guid cell2, int score) data)
        {
            var pos1 = _layoutManager.GetCellPosition(data.cell1);
            var pos2 = _layoutManager.GetCellPosition(data.cell2);
            if (pos1.HasValue && pos2.HasValue)
            {
                _visuals.ShowFloatingScoreForPair(pos1.Value, pos2.Value, -data.score, false);
            }
        }

        private void HandleLineScoreAdded((int lineIndex, int score) data)
        {
            _visuals.ShowFloatingScoreForLine(data.lineIndex, data.score, true);
        }

        private void HandleLineScoreUndone((int lineIndex, int score) data)
        {
            _visuals.ShowFloatingScoreForLine(data.lineIndex, -data.score, false);
        }
    }
}
