using System;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateCollected
    {
        public readonly AssetPair Pair;
        public readonly Money Price;
        public readonly LatestPrice Response;
        public readonly DateTime UtcCreated;
        public readonly IPublicPriceProvider Provider;

        public ExchangeRateCollected(IPublicPriceProvider provider, AssetPairNormalised pair, LatestPrice latestPrice)
        {
            Response = latestPrice;
            Provider = provider;
            Pair = pair.Normalised;
            Price = latestPrice.Price;
            UtcCreated = latestPrice.UtcCreated;
        }
    }
}