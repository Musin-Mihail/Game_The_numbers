using System;

namespace YandexGames
{
    [Serializable]
    public class ProductInfo
    {
        public string id;
        public string title;
        public string description;
        public string price;
        public string priceValue;
        public string priceCurrencyCode;
        public bool consumed = true;
    }

    [Serializable]
    public class ProductCatalogJson
    {
        public ProductInfo[] items;
    }
}
