using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.NovaExchange
{
    internal interface INovaExchangeApi
    {
        [Get("/market/info/{currencyPair}/")]
        Task<NovaExchangeSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/markets/")]
        Task<NovaExchangeSchema.TickerResponse> GetTickersAsync();
    }
}
