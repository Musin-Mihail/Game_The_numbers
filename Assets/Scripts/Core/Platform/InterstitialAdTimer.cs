using Core.Events;
using UnityEngine;
using YG;

namespace Core.Platform
{
    /// <summary>
    /// Вызывает показ межстраничной рекламы по событию onAddExistingNumbers.
    /// Логика кулдауна и времени показа рекламы управляется самим плагином YG2.
    /// </summary>
    public class InterstitialAdTimer : MonoBehaviour
    {
        private float _sessionStartTime;

        private void OnEnable()
        {
            _sessionStartTime = Time.realtimeSinceStartup;
            GlobalEvents.OnAddExistingNumbers += OnAddExistingNumbersTriggered;
        }

        private void OnDisable()
        {
            GlobalEvents.OnAddExistingNumbers -= OnAddExistingNumbersTriggered;
        }

        /// <summary>
        /// Вызывается при срабатывании события onAddExistingNumbers.
        /// Запрашивает показ рекламы через плагин.
        /// </summary>
        private void OnAddExistingNumbersTriggered()
        {
            if (Time.realtimeSinceStartup - _sessionStartTime < 180f) return;
            YG2.InterstitialAdvShow();
        }
    }
}