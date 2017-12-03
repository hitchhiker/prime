using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bleutrade
{
    internal interface IBleutradeApi
    {
        [Get("/public/getticker?market={currencyPair}")]
        Task<BleutradeSchema.BaseResponse<BleutradeSchema.TickerEntry[]>> GetTickerAsync([Path] string currencyPair);

        [Get("/public/getmarketsummaries")]
        Task<BleutradeSchema.BaseResponse<BleutradeSchema.MarketEntry[]>> GetMarketsAsync();

        [Get("/public/getmarketsummary?market={currencyPair}")]
        Task<BleutradeSchema.BaseResponse<BleutradeSchema.MarketEntry[]>> GetMarketAsync([Path] string currencyPair);
    }
}
