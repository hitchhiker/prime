namespace Prime.Core.Exchange.Rates
{
    public class ExchangeRateUpdatedMessage
    {
        public readonly ExchangeRateResult Result;

        public ExchangeRateUpdatedMessage(ExchangeRateResult rate)
        {
            Result = rate;
        }
    }
}