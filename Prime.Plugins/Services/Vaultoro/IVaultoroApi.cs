using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Vaultoro
{
    internal interface IVaultoroApi
    {
        [Get("/markets")]
        Task<VaultoroSchema.MarketResponse> GetMarketsAsync();
    }
}
