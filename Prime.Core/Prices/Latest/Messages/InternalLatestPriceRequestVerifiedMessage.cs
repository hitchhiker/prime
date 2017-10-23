using Prime.Common.Exchange.Rates;

namespace Prime.Core
{
    internal class InternalLatestPriceRequestVerifiedMessage
    {
        public readonly LatestPriceRequest Request;

        public InternalLatestPriceRequestVerifiedMessage(LatestPriceRequest request)
        {
            Request = request;
        }
    }
}