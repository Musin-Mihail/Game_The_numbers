using System;
using YandexGames;

namespace Interfaces
{
    /// <summary>
    /// Интерфейс для взаимодействия с сервисами платформы (покупки, реклама).
    /// </summary>
    public interface IPlatformServices
    {
        event Action<string> OnPurchaseSuccess;
        event Action<string> OnPurchaseFailed;
        event Action<string> OnRewardVideoSuccess;
        event Action<bool> OnInterstitialClosed;

        void Purchase(string productId);
        void ShowRewardedAd(string rewardId);
        void ShowInterstitialAd();
        void ConsumePurchase(string productId);
        ProductInfo GetProduct(string productId);
    }
}
