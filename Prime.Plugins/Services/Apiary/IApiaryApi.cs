using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Apiary
{
    internal interface IApiaryApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<ApiarySchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/info")]
        Task<ApiarySchema.AssetPairsResponse> GetAssetPairsAsync();
    }
}
