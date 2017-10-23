using Prime.Core;
using Prime.Utility;

namespace Prime.Common.Exchange.Rates
{
    internal class LatestPriceSubscriptions
    {
        public readonly UniqueList<LatestPriceRequest> Requests = new UniqueList<LatestPriceRequest>();
    }
}