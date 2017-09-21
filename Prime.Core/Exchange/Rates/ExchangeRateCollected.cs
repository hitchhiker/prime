using System;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateCollected
    {
        public readonly AssetPair Pair;
        public readonly Money Price;
        public readonly DateTime UtcCreated;
        public readonly IPublicPriceProvider Provider;
        public readonly IPublicPriceProvider ProviderConversion;
        public bool IsConverted => ProviderConversion != null;

        public ExchangeRateCollected(IPublicPriceProvider provider, AssetPair pair, LatestPrice latestPrice, bool isReversed)
        {
            UtcCreated = latestPrice.UtcCreated;
            Provider = provider;
            Pair = pair;
            Price = isReversed ? new Money(1d / latestPrice.Price, pair.Asset1) : latestPrice.Price;
        }

        public ExchangeRateCollected(ExchangeRateRequest request, ExchangeRateCollected recent, ExchangeRateCollected other)
        {
            Pair = request.Pair;
            var p1 = request.IsConvertedPart1 ? recent : other;
            var p2 = request.IsConvertedPart1 ? other : recent;

            UtcCreated = recent.UtcCreated > other.UtcCreated ? other.UtcCreated : recent.UtcCreated;
            Provider = p1.Provider;
            ProviderConversion = p2.Provider;
            Price = new Money(p1.Price * p2.Price, Pair.Asset2);
        }
    }
}