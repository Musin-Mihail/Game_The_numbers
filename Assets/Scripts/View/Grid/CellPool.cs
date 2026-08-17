using System.Collections.Generic;
using UnityEngine;
using View.UI.Builder;

namespace View.Grid
{
    /// <summary>
    /// Реализует пул объектов для ячеек (Cell) для оптимизации производительности.
    /// </summary>
    public class CellPool : MonoBehaviour
    {
        private Transform _canvasTransform;
        private readonly Queue<Cell> _pooledCells = new();

        private void Awake()
        {
            var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include);
            foreach (var t in allTransforms)
            {
                if (t.name == UiIds.Content && t.parent != null && t.parent.name == UiIds.Viewport)
                {
                    _canvasTransform = t;
                    break;
                }
            }

            if (_canvasTransform == null)
            {
                Debug.LogError("[CellPool] Не удалось найти 'Content' внутри 'Viewport' на Canvas.");
                _canvasTransform = transform;
            }
        }

        /// <summary>
        /// Получает ячейку из пула или создает новую, если пул пуст.
        /// </summary>
        /// <returns>Экземпляр ячейки.</returns>
        public Cell GetCell()
        {
            Cell cell;
            if (_pooledCells.Count > 0)
            {
                cell = _pooledCells.Dequeue();
            }
            else
            {
                cell = WidgetFactory.CreateCell(_canvasTransform);
            }

            cell.transform.SetParent(_canvasTransform, false);
            cell.transform.SetAsFirstSibling();
            cell.gameObject.SetActive(true);
            return cell;
        }

        /// <summary>
        /// Возвращает ячейку в пул для повторного использования.
        /// </summary>
        /// <param name="cell">Ячейка для возврата.</param>
        public void ReturnCell(Cell cell)
        {
            if (!cell) return;
            cell.gameObject.SetActive(false);
            cell.ResetForPooling();
            _pooledCells.Enqueue(cell);
        }
    }
}