using Core;
using Core.Events;
using TMPro;
using UnityEngine;

namespace View.UI
{
    /// <summary>
    /// Отображает количество доступных действий игрока (отмена, добавление, подсказка) в UI.
    /// </summary>
    public class ActionCountersView : MonoBehaviour
    {
        private GameObject _undoCount;
        private GameObject _hintCount;
        private TextMeshProUGUI _undoCountText;
        private TextMeshProUGUI _hintCountText;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            _undoCount = transform.FindGameObjectInChildren("Obj_UndoCount");
            _hintCount = transform.FindGameObjectInChildren("Obj_HintCount");
            _undoCountText = transform.FindComponentInChildren<TextMeshProUGUI>("Txt_UndoCountText");
            _hintCountText = transform.FindComponentInChildren<TextMeshProUGUI>("Txt_HintCountText");
            var btnUndo = _undoCount?.GetComponent<UnityEngine.UI.Button>();
            btnUndo?.onClick.AddListener(() => GlobalEvents.OnUndoLastAction?.Invoke());
            var btnHint = _hintCount?.GetComponent<UnityEngine.UI.Button>();
            btnHint?.onClick.AddListener(() => GlobalEvents.OnRequestHint?.Invoke());
            var btnAdd = transform.FindComponentInChildren<UnityEngine.UI.Button>("NewLines");
            btnAdd?.onClick.AddListener(() => GlobalEvents.OnAddExistingNumbers?.Invoke());
            var btnMenu = transform.FindComponentInChildren<UnityEngine.UI.Button>("Menu");
            btnMenu?.onClick.AddListener(() => GlobalEvents.OnShowMenu?.Invoke());
            /*

            foreach (var t in allTransforms)
            {
                if (t.name == "Obj_UndoCount") _undoCount = t.gameObject;
                else if (t.name == "Obj_HintCount") _hintCount = t.gameObject;
                else if (t.name == "Txt_UndoCountText") _undoCountText = t.GetComponent<TextMeshProUGUI>();
                else if (t.name == "Txt_HintCountText") _hintCountText = t.GetComponent<TextMeshProUGUI>();
                else if (t.name == "NewLines" && t.parent != null && t.parent.name == "Buttons") btnAdd = t.GetComponent<UnityEngine.UI.Button>();
                else if (t.name == "Menu" && t.parent != null && t.parent.name == "Buttons") btnMenu = t.GetComponent<UnityEngine.UI.Button>();
            }

            var btnUndo = _undoCount?.GetComponent<UnityEngine.UI.Button>();
            btnUndo?.onClick.AddListener(() => GlobalEvents.OnUndoLastAction?.Invoke());

            var btnHint = _hintCount?.GetComponent<UnityEngine.UI.Button>();
            btnHint?.onClick.AddListener(() => GlobalEvents.OnRequestHint?.Invoke());

            */
        }

        private void OnEnable()
        {
            GlobalEvents.OnCountersChanged += UpdateCountersUI;
        }

        private void OnDisable()
        {
            GlobalEvents.OnCountersChanged -= UpdateCountersUI;
        }

        /// <summary>
        /// Обновляет текстовые поля с количеством действий.
        /// Если счетчик бесконечен (-1), соответствующий GameObject отключается.
        /// Если счетчик равен 0, отображается "+".
        /// </summary>
        /// <param name="data">Кортеж с количеством отмен и подсказок.</param>
        private void UpdateCountersUI((int undo, int hint) data)
        {
            var areCountersInfinite = data.undo == -1;

            if (areCountersInfinite)
            {
                if (_undoCount) _undoCount.SetActive(false);
                if (_hintCount) _hintCount.SetActive(false);
            }
            else
            {
                if (_undoCountText)
                {
                    _undoCountText.gameObject.SetActive(true);
                    _undoCountText.text = data.undo == 0 ? "+" : data.undo.ToString();
                }

                if (_hintCountText)
                {
                    _hintCountText.gameObject.SetActive(true);
                    _hintCountText.text = data.hint == 0 ? "+" : data.hint.ToString();
                }
            }
        }
    }
}