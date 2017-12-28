using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitsane
{
    internal interface IBitsaneApi
    {
        [Get("/ticker?pairs={currencyPair}")]
        Task<BitsaneSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker")]
        Task<BitsaneSchema.TickerResponse> GetTickersAsync();

        [Get("/orderbook?pair={currencyPair}")]
        Task<BitsaneSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
