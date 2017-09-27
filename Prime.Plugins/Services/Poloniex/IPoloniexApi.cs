using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Poloniex
{
    internal interface IPoloniexApi
    {
        [Post("/tradingApi")]
        Task<Dictionary<string, decimal>> GetBalancesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<PoloniexSchema.BalancesDetailedResponse> GetBalancesDetailedAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Get("/public?command=returnTicker")]
        Task<PoloniexSchema.TickerResponse> GetTickerAsync();
    }
}
