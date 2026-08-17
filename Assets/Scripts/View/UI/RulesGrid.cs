using UnityEngine;
using UnityEngine.UI;
using View.Grid;
using View.UI.Builder;

namespace View.UI
{
    /// <summary>
    /// Генерирует и отображает демонстрационную сетку в окне правил,
    /// подсвечивая примеры допустимых пар.
    /// </summary>
    public class RulesGrid : MonoBehaviour
    {
        private RectTransform _gridContainer;
        private const int GridSize = 5;

        private Cell[,] _cells;
        private int[,] _gridNumbers;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            var container = transform.Find(UiIds.RulesGridContainer);
            if (container != null)
            {
                _gridContainer = container.GetComponent<RectTransform>();
                return;
            }

            var allTransforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            foreach (var t in allTransforms)
            {
                if (t.name != UiIds.RulesGridContainer) continue;
                _gridContainer = t.GetComponent<RectTransform>();
                break;
            }
        }

        /// <summary>
        /// Генерирует демонстрационную сетку с предопределенными числами.
        /// </summary>
        public void GenerateGrid()
        {
            ClearGrid();
            _cells = new Cell[GridSize, GridSize];
            _gridNumbers = new int[GridSize, GridSize];
            _gridNumbers[2, 1] = 7;
            _gridNumbers[2, 2] = 7;
            _gridNumbers[0, 3] = 3;
            _gridNumbers[1, 3] = 7;
            _gridNumbers[2, 4] = 5;
            _gridNumbers[3, 0] = 5;
            _gridNumbers[0, 0] = 1;
            _gridNumbers[4, 4] = 1;
            FillRemainingCells();
            InstantiateGrid();
            HighlightPairs();
        }

        private void FillRemainingCells()
        {
            int[] fillerNumbers = { 2, 4, 6, 8, 9 };
            var fillerIndex = 0;
            for (var y = 0; y < GridSize; y++)
            {
                for (var x = 0; x < GridSize; x++)
                {
                    if (_gridNumbers[y, x] != 0) continue;
                    _gridNumbers[y, x] = fillerNumbers[fillerIndex % fillerNumbers.Length];
                    fillerIndex++;
                }
            }
        }

        private void InstantiateGrid()
        {
            if (!_gridContainer)
            {
                Debug.LogError("Контейнер сетки правил не назначен.");
                return;
            }

            var cellSize = Core.GameConstants.CellSize;
            const float spacing = 5f;

            var totalWidth = GridSize * cellSize + (GridSize - 1) * spacing;
            var totalHeight = GridSize * cellSize + (GridSize - 1) * spacing;

            var layoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = totalHeight + 20f;
            layoutElement.minWidth = totalWidth;

            _gridContainer.anchorMin = new Vector2(0.5f, 0.5f);
            _gridContainer.anchorMax = new Vector2(0.5f, 0.5f);
            _gridContainer.pivot = new Vector2(0.5f, 0.5f);
            _gridContainer.sizeDelta = new Vector2(totalWidth, totalHeight);
            _gridContainer.anchoredPosition = Vector2.zero;

            var startX = -totalWidth / 2f + cellSize / 2f;
            var startY = totalHeight / 2f - cellSize / 2f;

            for (var y = 0; y < GridSize; y++)
            {
                for (var x = 0; x < GridSize; x++)
                {
                    var cell = WidgetFactory.CreateCell(_gridContainer);
                    _cells[y, x] = cell;

                    cell.text.text = _gridNumbers[y, x].ToString();
                    cell.SetVisualState(true);

                    var rectTransform = cell.GetComponent<RectTransform>();
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.anchoredPosition = new Vector2(startX + x * (cellSize + spacing), startY - y * (cellSize + spacing));
                }
            }
        }

        private void HighlightPairs()
        {
            HighlightCell(2, 1, UiTheme.RulesSameNumber);
            HighlightCell(2, 2, UiTheme.RulesSameNumber);
            HighlightCell(0, 3, UiTheme.RulesSumIsTen);
            HighlightCell(1, 3, UiTheme.RulesSumIsTen);
            HighlightCell(2, 4, UiTheme.RulesLineWrap);
            HighlightCell(3, 0, UiTheme.RulesLineWrap);
            HighlightCell(0, 0, UiTheme.RulesFirstAndLast);
            HighlightCell(4, 4, UiTheme.RulesFirstAndLast);
        }

        private void HighlightCell(int y, int x, Color color)
        {
            if (y < GridSize && x < GridSize && _cells[y, x])
            {
                _cells[y, x].GetComponent<Image>().color = color;
            }
        }

        private void ClearGrid()
        {
            if (_gridContainer != null)
            {
                foreach (Transform child in _gridContainer)
                {
                    Destroy(child.gameObject);
                }
            }

            _cells = null;
        }
    }
}
