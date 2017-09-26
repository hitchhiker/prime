using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
