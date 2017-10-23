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
        public Asset AssetConvert { get; }
        public Money Price { get; }
        public Money PriceConvert1 { get; }
        public Money PriceConvert2 { get; }

        public bool IsConverted => ProviderConversion != null;

        public LatestPriceResultMessage(IPublicPriceProvider provider, AssetPair pair, LatestPrice latestPrice, bool isReversed)
        {
            UtcCreated = latestPrice.UtcCreated;
            Provider = provider;
            Pair = pair;
            Price = isReversed ? new Money(1d / latestPrice.Price, pair.Asset1) : latestPrice.Price;
        }

        public LatestPriceResultMessage(AssetPair pair, bool isPart1, LatestPriceResultMessage recent, LatestPriceResultMessage other)
        {
            Pair = pair;
            var p1 = isPart1 ? recent : other;
            var p2 = isPart1 ? other : recent;

            UtcCreated = recent.UtcCreated > other.UtcCreated ? other.UtcCreated : recent.UtcCreated;
            Provider = p1.Provider;
            ProviderConversion = p2.Provider;
            Price = new Money(p1.Price * p2.Price, Pair.Asset2);
            PriceConvert1 = p1.Price;
            PriceConvert2 = p2.Price;
            AssetConvert = p1.Pair.Asset2;
        }

        public bool IsMatch(LatestPriceResultMessage request)
        {
            if (request == null)
                return false;

            return Pair.Equals(request.Pair) && Provider == request.Provider && ProviderConversion == request.ProviderConversion;
        }
    }
}