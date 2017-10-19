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
        /// <param name="symbol">Currency pair code</param>
        /// <param name="limit">The maximum number or records to return.</param>
        /// <returns></returns>
        [Get("/v1/depth?symbol={symbol}&limit={limit}")]
        Task<BinanceSchema.OrderBookResponse> GetOrderBook([Path] string symbol, [Path] int limit = 100);

        /// <summary>
        /// Returns user account information.
        /// For more details see https://www.binance.com/restapipub.html#user-content-account-endpoints.
        /// </summary>
        /// <returns></returns>
        [Get("/v3/account")]
        Task<BinanceSchema.UserInformationResponse> GetAccountInformation();
    }
}
