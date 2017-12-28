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
        Task<BleutradeSchema.BaseResponse<BleutradeSchema.TickerEntryResponse[]>> GetTickerAsync([Path] string currencyPair);

        [Get("/public/getmarketsummaries")]
        Task<BleutradeSchema.BaseResponse<BleutradeSchema.MarketEntryResponse[]>> GetMarketsAsync();

        [Get("/public/getmarketsummary?market={currencyPair}")]
        Task<BleutradeSchema.BaseResponse<BleutradeSchema.MarketEntryResponse[]>> GetMarketAsync([Path] string currencyPair);

        [Get("/public/getorderbook?market={currencyPair}&type=all")]
        Task<BleutradeSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
