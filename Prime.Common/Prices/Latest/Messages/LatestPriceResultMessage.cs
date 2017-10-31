using System;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Utility;

namespace Prime.Common.Exchange.Rates
{
    public class LatestPriceResultMessage
    {
        public readonly IPublicPriceProvider Provider;
        public readonly IPublicPriceProvider ProviderConversion;

        public DateTime UtcCreated { get; }
        public AssetPair Pair { get; }
        public Money Price { get; }

        public Asset AssetConvert { get; }
        public Money PriceConvert1 { get; }
        public Money PriceConvert2 { get; }

        public bool IsConverted => ProviderConversion != null;

        public LatestPriceResultMessage(IPublicPriceProvider provider, AssetPair pair, LatestPrice latestPrice)
        {
            UtcCreated = latestPrice.UtcCreated;
            Provider = provider;
            Pair = pair;
            Price = latestPrice.Price;
        }

        public LatestPriceResultMessage(AssetPair originalPair, LatestPrice price1, LatestPrice price2, IPublicPriceProvider provider, IPublicPriceProvider providerConvert)
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

        public bool IsMatch(LatestPriceResultMessage request)
        {
            if (request == null)
                return false;

            return Pair.Equals(request.Pair) && Provider == request.Provider && ProviderConversion == request.ProviderConversion;
        }
    }
}