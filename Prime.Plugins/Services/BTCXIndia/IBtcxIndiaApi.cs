using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BTCXIndia
{
    internal interface IBtcxIndiaApi
    {
        [Get("/ticker")]
        Task<BtcxIndiaSchema.TickerResponse> GetTickersAsync();
    }
}
