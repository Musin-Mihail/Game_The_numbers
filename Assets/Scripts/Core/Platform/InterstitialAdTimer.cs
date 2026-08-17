using Core.Events;
using UnityEngine;
using YandexGames;

namespace Core.Platform
{
    /// <summary>
    /// Показ interstitial при добавлении чисел: не раньше 180 с сессии и не чаще 60 с после успеха.
    /// </summary>
    public class InterstitialAdTimer : MonoBehaviour
    {
        private float _sessionStartTime;
        private float? _lastSuccessfulShowTime;

        private void OnEnable()
        {
            _sessionStartTime = Time.realtimeSinceStartup;
            GlobalEvents.OnAddExistingNumbers += OnAddExistingNumbersTriggered;
            YandexGamesSdk.InterstitialClosed += OnInterstitialClosed;
        }

        private void OnDisable()
        {
            GlobalEvents.OnAddExistingNumbers -= OnAddExistingNumbersTriggered;
            YandexGamesSdk.InterstitialClosed -= OnInterstitialClosed;
        }

        private void OnAddExistingNumbersTriggered()
        {
            var sessionElapsed = Time.realtimeSinceStartup - _sessionStartTime;
            float? sinceSuccess = null;
            if (_lastSuccessfulShowTime.HasValue)
            {
                sinceSuccess = Time.realtimeSinceStartup - _lastSuccessfulShowTime.Value;
            }

            if (!InterstitialAdPolicy.CanShow(sessionElapsed, sinceSuccess))
            {
                return;
            }

            YandexGamesSdk.ShowInterstitial();
        }

        private void OnInterstitialClosed(bool wasShown)
        {
            if (!wasShown)
            {
                return;
            }

            _lastSuccessfulShowTime = Time.realtimeSinceStartup;
        }
    }
}
