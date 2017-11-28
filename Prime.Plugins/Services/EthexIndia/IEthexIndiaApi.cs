using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.EthexIndia
{
    internal interface IEthexIndiaApi
    {
        [Get("/ticker")]
        Task<EthexIndiaSchema.TickerResponse[]> GetTickersAsync();
    }
}
