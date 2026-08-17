using Core.Events;
using Core;
using Interfaces;
using Localization;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YandexGames;

namespace Core.Shop
{
    /// <summary>
    /// Управляет UI элементами магазина, отображая информацию о товарах и их статусе.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        private Button _purchaseButton;
        private TextMeshProUGUI _priceText;
        private ProductInfo _productInfo;
        private ActionCountersModel _actionCountersModel;
        private LocalizationManager _localizationManager;
        private IPlatformServices _platformServices;

        private void Awake()
        {
            BindUI();
        }

        private void BindUI()
        {
            _purchaseButton = transform.FindComponentInChildren<Button>("DisabledCounters");
            if (_purchaseButton != null)
            {
                var cost = _purchaseButton.transform.Find("Cost");
                if (cost != null) _priceText = cost.Find("Text")?.GetComponent<TextMeshProUGUI>();
            }
        }

        public void Initialize(ActionCountersModel actionCountersModel)
        {
            _actionCountersModel = actionCountersModel;
            _localizationManager = ServiceProvider.GetService<LocalizationManager>();
            _platformServices = ServiceProvider.GetService<IPlatformServices>();

            if (YandexGamesSdk.IsReady)
            {
                InitializeShopProduct();
            }
        }

        private void OnEnable()
        {
            GlobalEvents.OnYandexSDKInitialized += InitializeShopProduct;
            if (_platformServices != null)
            {
                _platformServices.OnPurchaseSuccess += HandlePurchaseSuccess;
            }
            LocalizationManager.OnLanguageChanged += HandleLanguageChanged;
        }

        private void OnDisable()
        {
            GlobalEvents.OnYandexSDKInitialized -= InitializeShopProduct;
            if (_platformServices != null)
            {
                _platformServices.OnPurchaseSuccess -= HandlePurchaseSuccess;
            }
            LocalizationManager.OnLanguageChanged -= HandleLanguageChanged;
        }

        /// <summary>
        /// Обрабатывает событие смены языка, обновляя UI магазина.
        /// </summary>
        private void HandleLanguageChanged()
        {
            UpdateProductUI();
        }

        /// <summary>
        /// Инициализирует информацию о продукте после загрузки Yandex SDK.
        /// </summary>
        private void InitializeShopProduct()
        {
            if (_actionCountersModel == null)
            {
                return;
            }

            if (_localizationManager == null)
            {
                _localizationManager = ServiceProvider.GetService<LocalizationManager>();
                if (_localizationManager == null) return;
            }

            _platformServices ??= ServiceProvider.GetService<IPlatformServices>();
            _platformServices?.ConsumePurchase(GameConstants.DisableCountersProductId);
            _productInfo = _platformServices?.GetProduct(GameConstants.DisableCountersProductId);
            UpdateProductUI();
        }

        private void UpdateProductUI()
        {
            if (_actionCountersModel == null) return;
            if (_localizationManager == null)
            {
                _localizationManager = ServiceProvider.GetService<LocalizationManager>();
                if (_localizationManager == null) return;
            }

            if (_actionCountersModel.AreCountersDisabled)
            {
                SetProductAsPurchased();
            }
            else
            {
                if (_productInfo != null)
                {
                    SetProductAsAvailable();
                }
                else
                {
                    if (_priceText) _priceText.text = _localizationManager.Get("shopProductNotFound");
                    Debug.LogError($"Ошибка ShopManager: Товар с ID '{GameConstants.DisableCountersProductId}' не найден.");
                    if (_purchaseButton) _purchaseButton.interactable = false;
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие успешной покупки (как новой, так и необработанной).
        /// </summary>
        /// <param name="purchasedId">ID купленного товара.</param>
        private void HandlePurchaseSuccess(string purchasedId)
        {
            if (purchasedId != GameConstants.DisableCountersProductId) return;
            Debug.Log($"Покупка '{purchasedId}' успешно обработана. Обновление UI.");
            UpdateProductUI();
        }

        /// <summary>
        /// Обновляет UI, чтобы показать товар как доступный для покупки.
        /// </summary>
        private void SetProductAsAvailable()
        {
            if (_priceText)
                _priceText.text = _productInfo.price;
            if (!_purchaseButton) return;
            _purchaseButton.interactable = true;
            _purchaseButton.onClick.RemoveAllListeners();
            _purchaseButton.onClick.AddListener(() => GlobalEvents.OnRequestDisableCounters?.Invoke());
        }

        /// <summary>
        /// Обновляет UI, чтобы показать товар как купленный.
        /// </summary>
        private void SetProductAsPurchased()
        {
            if (_priceText)
                _priceText.text = _localizationManager.Get("shopPurchased");
            if (!_purchaseButton) return;
            _purchaseButton.interactable = false;
            _purchaseButton.onClick.RemoveAllListeners();
        }
    }
}