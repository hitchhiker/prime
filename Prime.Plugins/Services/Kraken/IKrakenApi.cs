using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Plugins.Services.Kraken;
using RestEase;

namespace plugins
{
    internal interface IKrakenApi
    {
        
        [Post("/private/Balance")]
        Task<KrakenSchema.BalancesResponse> GetBalancesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
