using Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TEST
{
    /// <summary>
    /// Отвечает за обработку ввода с клавиатуры для вызова подсказки.
    /// </summary>
    public class HintInputController : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.hKey.wasPressedThisFrame) return;
            Debug.Log("Клавиша H нажата, вызываем onRequestHint.");
            GlobalEvents.OnRequestHint?.Invoke();
        }
    }
}
