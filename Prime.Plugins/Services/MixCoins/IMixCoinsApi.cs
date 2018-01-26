using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.MixCoins
{
    internal interface IMixCoinsApi
    {
        [Get("/ticker?market={currencyPair}")]
        Task<MixCoinsSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/depth?market={currencyPair}")]
        Task<MixCoinsSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
