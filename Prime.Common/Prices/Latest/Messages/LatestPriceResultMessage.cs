using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Utility;
using Prime.Utility.Misc;

namespace Prime.Common.Exchange.Rates
{
    public class LatestPriceResultMessage
    {
        public DateTime UtcCreated { get; }

        public AssetPair Pair => MarketPrice.Pair;
        public Money Price => MarketPrice.Price;

        public readonly IPublicPricingProvider Provider;
        public readonly MarketPrice MarketPrice;
        public readonly IPublicPricingProvider ProviderConversion;

        public Asset AssetIntermediary { get; }
        public MarketPrice MarketPrice1 { get; }
        public MarketPrice MarketPrice2 { get; }

        public bool IsConverted => ProviderConversion != null;

        public LatestPriceResultMessage(IPublicPricingProvider provider, MarketPrice marketPrice)
        {
            UtcCreated = marketPrice.UtcCreated;
            Provider = provider;
            MarketPrice = marketPrice;
        }

        public LatestPriceResultMessage(MarketPrice price, Asset intermediary, MarketPrice price1, MarketPrice price2, IPublicPricingProvider provider, IPublicPricingProvider providerConvert)
        {
            UtcCreated = price1.UtcCreated > price2.UtcCreated ? price2.UtcCreated : price1.UtcCreated;
            Provider = provider;
            MarketPrice = price;

            MarketPrice1 = price1;
            MarketPrice2 = price2;

            AssetIntermediary = intermediary;
            ProviderConversion = providerConvert;
        }

        public bool IsSimilarRequest(LatestPriceResultMessage request)
        {
            if (request == null)
                return false;

            return Pair.Equals(request.Pair) && AssetIntermediary.EqualOrBothNull(request.AssetIntermediary) && Provider.EqualOrBothNull(request.Provider) && ProviderConversion.EqualOrBothNull(request.ProviderConversion);
        }
    }
}