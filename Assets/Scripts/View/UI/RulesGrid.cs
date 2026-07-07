using UnityEngine;
using UnityEngine.UI;
using View.Grid;

namespace View.UI
{
    /// <summary>
    /// Генерирует и отображает демонстрационную сетку в окне правил,
    /// подсвечивая примеры допустимых пар.
    /// </summary>
    public class RulesGrid : MonoBehaviour
    {
        private GameObject _cellPrefab;
        private RectTransform _gridContainer;
        private const int GridSize = 5;

        [Header("Цвета подсветки")]
        [SerializeField] private Color sameNumberColor = Color.yellow;
        [SerializeField] private Color sumIsTenColor = Color.cyan;
        [SerializeField] private Color lineWrapColor = Color.magenta;
        [SerializeField] private Color firstAndLastColor = Color.green;

        private Cell[,] _cells;
        private int[,] _gridNumbers;

        private void Awake()
        {
            BindUI();
            _cellPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Cell");
        }

        private void BindUI()
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in allTransforms)
            {
                if (t.name == "Container_Grid")
                {
                    _gridContainer = t.GetComponent<RectTransform>();
                    break;
                }
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
            if (!_cellPrefab || !_gridContainer)
            {
                Debug.LogError("Префаб ячейки или контейнер сетки не назначены.");
                return;
            }

            var cellSize = _cellPrefab.GetComponent<RectTransform>().sizeDelta.x;
            const float spacing = 5f;
            
            float totalWidth = GridSize * cellSize + (GridSize - 1) * spacing;
            float totalHeight = GridSize * cellSize + (GridSize - 1) * spacing;

            // Заставляем компонент резервировать место в VerticalLayoutGroup, чтобы текст не наезжал на сетку
            var layoutElement = GetComponent<UnityEngine.UI.LayoutElement>();
            if (layoutElement == null) layoutElement = gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
            layoutElement.minHeight = totalHeight + 20f;
            layoutElement.minWidth = totalWidth;

            // Центрируем контейнер математически строго
            _gridContainer.anchorMin = new Vector2(0.5f, 0.5f);
            _gridContainer.anchorMax = new Vector2(0.5f, 0.5f);
            _gridContainer.pivot = new Vector2(0.5f, 0.5f);
            _gridContainer.sizeDelta = new Vector2(totalWidth, totalHeight);
            _gridContainer.anchoredPosition = Vector2.zero;

            float startX = -totalWidth / 2f + cellSize / 2f;
            float startY = totalHeight / 2f - cellSize / 2f;

            for (var y = 0; y < GridSize; y++)
            {
                for (var x = 0; x < GridSize; x++)
                {
                    // false гарантирует, что масштаб и позиция префаба не сломаются
                    var cellGo = Instantiate(_cellPrefab, _gridContainer, false);
                    var cell = cellGo.GetComponent<Cell>();
                    _cells[y, x] = cell;

                    cell.text.text = _gridNumbers[y, x].ToString();
                    cell.SetVisualState(true);

                    var rectTransform = cell.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(startX + x * (cellSize + spacing), startY - y * (cellSize + spacing));
                }
            }
        }

        private void HighlightPairs()
        {
            HighlightCell(2, 1, sameNumberColor);
            HighlightCell(2, 2, sameNumberColor);
            HighlightCell(0, 3, sumIsTenColor);
            HighlightCell(1, 3, sumIsTenColor);
            HighlightCell(2, 4, lineWrapColor);
            HighlightCell(3, 0, lineWrapColor);
            HighlightCell(0, 0, firstAndLastColor);
            HighlightCell(4, 4, firstAndLastColor);
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