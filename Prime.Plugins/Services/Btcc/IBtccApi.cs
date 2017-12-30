using RestEase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Btcc
{
    internal interface IBtccApi
    {
        [Get("/ticker?symbol={currencyPair}")]
        Task<BtccSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/orderbook?symbol={currencyPair}&limit={limit}")]
        Task<BtccSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int limit);
    }
}
