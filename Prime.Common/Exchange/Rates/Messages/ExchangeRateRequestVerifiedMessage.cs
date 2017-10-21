namespace Prime.Common.Exchange.Rates
{
    public class ExchangeRateRequestVerifiedMessage
    {
        public readonly LatestPriceRequest Request;

        public ExchangeRateRequestVerifiedMessage(LatestPriceRequest request)
        {
            Request = request;
        }
    }
}