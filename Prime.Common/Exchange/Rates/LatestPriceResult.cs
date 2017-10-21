using System;

namespace Prime.Common.Exchange.Rates
{
    public class LatestPriceResult
    {
        public readonly IPublicPriceProvider Provider;
        public readonly IPublicPriceProvider ProviderConversion;
        public readonly LatestPriceRequest Request;

        public DateTime UtcCreated { get; }
        public AssetPair Pair { get; }
        public Asset AssetConvert { get; }
        public Money Price { get; }
        public Money PriceConvert1 { get; }
        public Money PriceConvert2 { get; }

        public bool IsConverted => ProviderConversion != null;

        public LatestPriceResult(LatestPriceRequest request, IPublicPriceProvider provider, AssetPair pair, LatestPrice latestPrice, bool isReversed)
        {
            Request = request;
            UtcCreated = latestPrice.UtcCreated;
            Provider = provider;
            Pair = pair;
            Price = isReversed ? new Money(1d / latestPrice.Price, pair.Asset1) : latestPrice.Price;
        }

        public LatestPriceResult(LatestPriceRequest request, LatestPriceResult recent, LatestPriceResult other)
        {
            Request = request;
            Pair = request.Pair;
            var p1 = request.IsConvertedPart1 ? recent : other;
            var p2 = request.IsConvertedPart1 ? other : recent;

            UtcCreated = recent.UtcCreated > other.UtcCreated ? other.UtcCreated : recent.UtcCreated;
            Provider = p1.Provider;
            ProviderConversion = p2.Provider;
            Price = new Money(p1.Price * p2.Price, Pair.Asset2);
            PriceConvert1 = p1.Price;
            PriceConvert2 = p2.Price;
            AssetConvert = p1.Pair.Asset2;
        }

        public bool IsMatch(LatestPriceResult request)
        {
            if (request == null)
                return false;

            return Pair.Equals(request.Pair) && Provider == request.Provider && ProviderConversion == request.ProviderConversion;
        }
    }
}