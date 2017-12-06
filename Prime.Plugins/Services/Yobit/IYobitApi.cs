using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Yobit
{
    internal interface IYobitApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<YobitSchema.AllTickersResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/info")]
        Task<YobitSchema.AssetPairsResponse> GetAssetPairsAsync();
    }
}
