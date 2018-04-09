namespace Prime.Common
{
    /// <summary>
    /// Base class for API providers responses.
    /// </summary>
    public abstract class ResponseModelBase : ModelBase
    {
        /// <summary>
        /// The number by which the rate limiter should be increased.
        /// Usually represents the number of API endpoint calls, but sometimes 1 call can affect exchange rate limiter more than by 1.
        /// For example: Binance "All orders" endpoint affects rate limiter by 5.
        /// (https://github.com/binance-exchange/binance-official-api-docs/blob/master/rest-api.md#all-orders-user_data)
        /// </summary>
        public int ApiHitsCount { get; set; } = 1;

        /// <summary>
        /// Increases API hits count by 1.
        /// </summary>
        public void ApiHit()
        {
            ApiHitsCount++;
        }
    }

    /// <summary>
    /// Generic class for in-provider response classes. It is used to not create new classes for each internal response object which should contain the number of API hits.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ResponseModelBase<TResult> : ResponseModelBase
    {
        public ResponseModelBase(TResult result)
        {
            Result = result;
        }

        public TResult Result { get; set; }
    }
}