using System.Collections.Generic;
using Core;
using Core.Events;
using Model;
using UnityEngine;
using View.Grid;

namespace View.UI
{
    /// <summary>
    /// Управляет отображением верхней строки с числами, дублирующими числа на сетке.
    /// </summary>
    public class HeaderNumberDisplay : MonoBehaviour
    {
        private GameObject _cellPrefab;
        private RectTransform _container;

        private readonly List<Cell> _topLineCells = new();

        private void Awake()
        {
            _cellPrefab = Resources.Load<GameObject>("Prefabs/Prefab_Cell");
            BindUI();
        }

        private void BindUI()
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in allTransforms)
            {
                if (t.name == "Container" && t.parent != null && t.parent.name == "GameSpace")
                {
                    _container = t.GetComponent<RectTransform>();
                    break;
                }
            }
        }

        private void OnEnable()
        {
            GlobalEvents.OnToggleTopLine += SetContainerActive;
        }

        private void Start()
        {
            if (!_cellPrefab)
            {
                Debug.LogError("Ошибка: 'Prefab_Cell' не найден в Resources/Prefabs!", this);
                enabled = false;
                return;
            }
            
            CreateLineDisplay();
        }

        private void OnDisable()
        {
            GlobalEvents.OnToggleTopLine -= SetContainerActive;
        }

        private void SetContainerActive(bool isActive)
        {
            if (_container)
            {
                _container.gameObject.SetActive(isActive);
            }
        }

        private void CreateLineDisplay()
        {
            if (!_cellPrefab)
            {
                Debug.LogError("Ошибка: '_cellPrefab' не загружен в HeaderNumberDisplay!", this);
                return;
            }

            for (var i = 0; i < GameConstants.QuantityByWidth; i++)
            {
                var cellGo = Instantiate(_cellPrefab, _container, false);
                var cell = cellGo.GetComponent<Cell>();

                cell.SetSelected(false);
                cell.SetVisualState(false);
                _topLineCells.Add(cell);
                cellGo.SetActive(true);
                var rectTransform = cell.TargetRectTransform;
                rectTransform.anchoredPosition = new Vector2(GameConstants.CellSize * i + GameConstants.Indent / 2f, -GameConstants.Indent / 2f);
            }
        }

        /// <summary>
        /// Обновляет отображаемые числа в верхней строке.
        /// </summary>
        /// <param name="numbers">Список чисел для отображения.</param>
        public void UpdateDisplayedNumbers(List<int> numbers)
        {
            for (var i = 0; i < _topLineCells.Count; i++)
            {
                if (i >= numbers.Count) continue;
                var cell = _topLineCells[i];
                var number = numbers[i];
                if (cell.Number != number)
                {
                    cell.text.text = number.ToString();
                }

                var isActive = number != 0;
                cell.SetVisualState(isActive);
            }
        }
    }
}