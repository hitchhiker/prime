using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Binance
{
    internal interface IBinanceApi
    {
        [Get("/v1/ticker/allPrices")]
        Task<BinanceSchema.LatestPricesResponse> GetSymbolPriceTicker();

        /// <summary>
        /// Maximum number of order book records is 100.
        /// </summary>
        /// <param name="symbol">Currency pair code.</param>
        /// <param name="limit">The maximum number or records to return.</param>
        /// <returns>List of order book entries.</returns>
        [Get("/v1/depth?symbol={symbol}&limit={limit}")]
        Task<BinanceSchema.OrderBookResponse> GetOrderBook([Path] string symbol, [Path] int limit = 100);

        /// <summary>
        /// Gets user account information.
        /// For more details see https://www.binance.com/restapipub.html#user-content-account-endpoints.
        /// </summary>
        /// <returns>Information about user account.</returns>
        [Get("/v3/account")]
        Task<BinanceSchema.UserInformationResponse> GetAccountInformation();

        /// <summary>
        /// Get OHLC data.
        /// </summary>
        /// <param name="currency">Currency pair code being queried.</param>
        /// <param name="interval">Time interval.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        /// <param name="limit">Limits the number of records to be returned.</param>
        /// <returns>Array of decimals that represent OHLC record.</returns>
        [Get("/v1/klines?symbol={currency}&interval={interval}")]
        Task<BinanceSchema.CandlestickResponse> GetCandlestickBars([Path] string currency, [Path] string interval, [Query] long? startTime = null, [Query] long? endTime = null, [Query] int? limit = null);
    }
}
