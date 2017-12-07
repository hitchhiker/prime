using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Wex
{
    internal interface IWexApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<WexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/info")]
        Task<WexSchema.AllAssetsResponse> GetAssetPairsAsync();
    }
}
