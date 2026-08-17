using System.Collections.Generic;
using Core;
using Core.Events;
using UnityEngine;
using View.Grid;
using View.UI.Builder;

namespace View.UI
{
    /// <summary>
    /// Управляет отображением верхней строки с числами, дублирующими числа на сетке.
    /// </summary>
    public class HeaderNumberDisplay : MonoBehaviour
    {
        private RectTransform _container;

        private readonly List<Cell> _topLineCells = new();

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            foreach (var t in allTransforms)
            {
                if (t.name == UiIds.HeaderContainer && t.parent != null && t.parent.name == UiIds.GameSpace)
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
            if (!_container)
            {
                Debug.LogError("Ошибка: контейнер верхней строки не найден в HeaderNumberDisplay!", this);
                return;
            }

            for (var i = 0; i < GameConstants.QuantityByWidth; i++)
            {
                var cell = WidgetFactory.CreateCell(_container);

                cell.SetSelected(false);
                cell.SetVisualState(false);
                _topLineCells.Add(cell);
                cell.gameObject.SetActive(true);
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