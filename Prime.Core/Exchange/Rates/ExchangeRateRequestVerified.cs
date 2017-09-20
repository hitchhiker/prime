namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateRequestVerifiedMessage
    {
        public readonly ExchangeRateRequest Request;

        public ExchangeRateRequestVerifiedMessage(ExchangeRateRequest request)
        {
            Request = request;
        }
    }
}