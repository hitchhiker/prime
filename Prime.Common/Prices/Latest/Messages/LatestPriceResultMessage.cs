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
        public AssetPair Pair { get; }
        public Money Price { get; }

        public readonly IPublicPriceSuper Provider;
        public readonly IPublicPriceSuper ProviderConversion;

        public Asset AssetConvert { get; }
        public Money PriceConvert1 { get; }
        public Money PriceConvert2 { get; }

        public bool IsConverted => ProviderConversion != null;

        public LatestPriceResultMessage(IPublicPriceSuper provider, LatestPrice latestPrice)
        {
            UtcCreated = latestPrice.UtcCreated;
            Provider = provider;
            Pair = latestPrice.Pair;
            Price = latestPrice.Price;
        }

        public LatestPriceResultMessage(AssetPair originalPair, LatestPrice price1, LatestPrice price2, IPublicPriceSuper provider, IPublicPriceSuper providerConvert)
        {
            Pair = originalPair;

            UtcCreated = price1.UtcCreated > price2.UtcCreated ? price2.UtcCreated : price1.UtcCreated;

            Price = new Money(price1.Price * price2.Reverse().Price, Pair.Asset2);

            PriceConvert1 = price1.Reverse().Price;
            PriceConvert2 = price2.Reverse().Price;

            AssetConvert = price1.Price.Asset;
            Provider = provider;
            ProviderConversion = providerConvert;
        }

        public bool IsSimilarRequest(LatestPriceResultMessage request)
        {
            if (request == null)
                return false;

            return Pair.Equals(request.Pair) && AssetConvert.EqualOrBothNull(request.AssetConvert) && Provider.EqualOrBothNull(request.Provider) && ProviderConversion.EqualOrBothNull(request.ProviderConversion);
        }
    }
}