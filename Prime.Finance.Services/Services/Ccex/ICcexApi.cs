﻿using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Ccex
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

        [Get("/t/api_pub.html?a=getorderbook&market={currencyPair}&type=both&depth={depth}")]
        Task<CcexSchema.OrderBookResponse> GetOrderBookAsync([Path(UrlEncode = false)] string currencyPair, [Path] int depth);
    }
}
