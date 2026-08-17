namespace YandexGames
{
    /// <summary>
    /// Правила показа interstitial: не в первые 180 с сессии, не чаще чем раз в 60 с после успешного показа.
    /// Отказ платформы не считается успехом и интервал не сдвигает.
    /// </summary>
    public static class InterstitialAdPolicy
    {
        public const float MinSessionSeconds = 180f;
        public const float MinIntervalSeconds = 60f;

        public static bool CanShow(float sessionElapsedSeconds, float? secondsSinceSuccessfulShow)
        {
            if (sessionElapsedSeconds < MinSessionSeconds)
            {
                return false;
            }

            if (secondsSinceSuccessfulShow.HasValue &&
                secondsSinceSuccessfulShow.Value < MinIntervalSeconds)
            {
                return false;
            }

            return true;
        }
    }
}
