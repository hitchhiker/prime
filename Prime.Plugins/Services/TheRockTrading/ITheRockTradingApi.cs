using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.TheRockTrading
{
    internal interface ITheRockTradingApi
    {
        [Get("/funds/{currencyPair}/ticker")]
        Task<TheRockTradingSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/funds/tickers")]
        Task<TheRockTradingSchema.AllTickersResponse> GetTickersAsync();

        [Get("/funds/{currencyPair}/orderbook")]
        Task<TheRockTradingSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
