using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.MercadoBitcoin
{
    internal interface IMercadoBitcoinApi
    {
        [Get("/{baseAsset}/ticker")]
        Task<MercadoBitcoinSchema.TickerResponse> GetTickerAsync([Path] string baseAsset);
    }
}
