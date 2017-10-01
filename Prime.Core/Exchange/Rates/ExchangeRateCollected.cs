using System;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateCollected
    {
        public readonly IPublicPriceProvider Provider;
        public readonly IPublicPriceProvider ProviderConversion;

        public DateTime UtcCreated { get; }
        public AssetPair Pair { get; }
        public Asset AssetConvert { get; }
        public Money Price { get; }
        public Money PriceConvert { get; }

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
            PriceConvert = p1.Price;
            AssetConvert = p1.Pair.Asset2;
        }
    }
}