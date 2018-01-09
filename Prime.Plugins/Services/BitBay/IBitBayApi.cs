using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitBay
{
    internal interface IBitBayApi
    {
        [Get("/{currencyPair}/ticker.json")]
        Task<BitBaySchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/{currencyPair}/orderbook.json")]
        Task<BitBaySchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
