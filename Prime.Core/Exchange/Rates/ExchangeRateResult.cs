using System;

namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateResult
    {
        public readonly AssetPair Pair;
        public readonly Money Price;
        public readonly LatestPrice Response;
        public readonly DateTime UtcCreated;

        public ExchangeRateResult(AssetPairNormalised pair, LatestPrice latestPrice)
        {
            Response = latestPrice;
            Pair = pair.Normalised;
            Price = latestPrice.Price;
            UtcCreated = latestPrice.UtcCreated;
        }
    }
}