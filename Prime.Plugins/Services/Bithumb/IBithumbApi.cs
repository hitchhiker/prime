using System.IO;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bithumb
{
    internal interface IBithumbApi
    {
        /// <summary>
        /// 
        /// See https://www.bithumb.com/u1/US127.
        /// </summary>
        /// <returns></returns>
        [Get("/public/ticker/ALL")]
        Task<BithumbSchema.TickersResponse> GetTickersAsync();

        /// <summary>
        /// Gets single ticker for specified currency code.
        /// See https://www.bithumb.com/u1/US127.
        /// </summary>
        /// <param name="currency">Currency which ticker is to be returned.</param>
        /// <returns>Ticker for specified currency.</returns>
        [Get("/public/ticker/{currency}")]
        Task<BithumbSchema.SingleTickerResponse> GetTickerAsync([Path] string currency);

        /// <summary>
        /// Gets order book for specified currency regarding KRW.
        /// </summary>
        /// <param name="currency">Currency which order book data is to be returned.</param>
        /// <param name="count">Number of records to be returned. Max is 50.</param>
        /// <returns>Order book data.</returns>
        [Get("/public/orderbook/{currency}")]
        Task<BithumbSchema.OrderBookResponse> GetOrderBookAsync([Path] string currency, [Query()] int? count = null);
    }
}