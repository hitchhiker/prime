using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bit2c
{
    internal interface IBit2CApi
    {
        [Get("/Exchanges/{currencyPair}/Ticker.json")]
        Task<Bit2CSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/Exchanges/{currencyPair}/orderbook.json")]
        Task<Bit2CSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
