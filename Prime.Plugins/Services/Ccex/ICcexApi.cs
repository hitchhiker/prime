using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Ccex
{
    internal interface ICcexApi
    {
        [Get("/t/{currencyPair}.json")]
        Task<CcexSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/t/prices.json")]
        Task<CcexSchema.AllTickersResponse> GetTickersAsync();

        [Get("/t/pairs.json")]
        Task<CcexSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/t/api_pub.html?a=getmarketsummaries")]
        Task<CcexSchema.VolumeResponse> GetVolumesAsync();
    }
}
