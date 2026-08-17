using System;
using Interfaces;
using YandexGames;

namespace Core.Platform
{
    /// <summary>
    /// Реализация платформенных сервисов (покупки, реклама) для Yandex Games.
    /// </summary>
    public class YandexPlatformService : IPlatformServices, IDisposable
    {
        public event Action<string> OnPurchaseSuccess;
        public event Action<string> OnPurchaseFailed;
        public event Action<string> OnRewardVideoSuccess;
        public event Action<bool> OnInterstitialClosed;

        public YandexPlatformService()
        {
            YandexGamesSdk.PurchaseSuccess += OnSdkPurchaseSuccess;
            YandexGamesSdk.PurchaseFailed += OnSdkPurchaseFailed;
            YandexGamesSdk.Rewarded += OnSdkRewarded;
            YandexGamesSdk.InterstitialClosed += OnSdkInterstitialClosed;
        }

        public void Dispose()
        {
            YandexGamesSdk.PurchaseSuccess -= OnSdkPurchaseSuccess;
            YandexGamesSdk.PurchaseFailed -= OnSdkPurchaseFailed;
            YandexGamesSdk.Rewarded -= OnSdkRewarded;
            YandexGamesSdk.InterstitialClosed -= OnSdkInterstitialClosed;
        }

        public void Purchase(string productId)
        {
            YandexGamesSdk.Purchase(productId);
        }

        public void ShowRewardedAd(string rewardId)
        {
            YandexGamesSdk.ShowRewarded(rewardId);
        }

        public void ShowInterstitialAd()
        {
            YandexGamesSdk.ShowInterstitial();
        }

        public void ConsumePurchase(string productId)
        {
            YandexGamesSdk.ConsumePurchase(productId);
        }

        public ProductInfo GetProduct(string productId)
        {
            return YandexGamesSdk.GetProduct(productId);
        }

        private void OnSdkPurchaseSuccess(string purchasedId)
        {
            OnPurchaseSuccess?.Invoke(purchasedId);
        }

        private void OnSdkPurchaseFailed(string failedId)
        {
            OnPurchaseFailed?.Invoke(failedId);
        }

        private void OnSdkRewarded(string rewardId)
        {
            OnRewardVideoSuccess?.Invoke(rewardId);
        }

        private void OnSdkInterstitialClosed(bool wasShown)
        {
            OnInterstitialClosed?.Invoke(wasShown);
        }
    }
}
