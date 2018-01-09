using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Luno
{
    internal interface ILunoApi
    {
        [Get("/ticker?pair={currencyPair}")]
        Task<LunoSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers")]
        Task<LunoSchema.AllTickersResponse> GetTickersAsync();

        [Get("/orderbook?pair={currencyPair}")]
        Task<LunoSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
