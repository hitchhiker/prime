using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.IndependentReserve
{
    internal interface IIndependentReserveApi
    {
        [Get("/GetMarketSummary?primaryCurrencyCode={primary}&secondaryCurrencyCode={secondary}")]
        Task<IndependentReserveSchema.TickerResponse> GetTickerAsync([Path] string primary, [Path]string secondary);

        [Get("/GetOrderBook?primaryCurrencyCode={primary}&secondaryCurrencyCode={secondary}")]
        Task<IndependentReserveSchema.OrderBookResponse> GetOrderBookAsync([Path] string primary, [Path]string secondary);
    }
}
