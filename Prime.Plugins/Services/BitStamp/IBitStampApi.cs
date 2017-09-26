using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using plugins.Services.BitStamp;
using RestEase;

namespace plugins
{
    internal interface IBitStampApi
    {
        [Get("ticker/{currency_pair}/")]
        Task<BitStampSchema.TickerResponse> GetTicker([Path("currency_pair")] string currencyPair);
    }
}
