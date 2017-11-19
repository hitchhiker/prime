using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Cryptopia
{
    internal interface ICryptopiaApi
    {
        [Get("/GetMarket/{assetPair}")]
        Task<CryptopiaSchema.MarketResponse> GetMarketAsync([Path] string assetPair);

        [Get("/GetMarkets")]
        Task<CryptopiaSchema.MarketResponse> GetAssetPairsAsync();
    }
}
