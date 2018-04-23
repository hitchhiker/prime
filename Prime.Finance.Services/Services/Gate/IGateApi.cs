﻿using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Gate
{
    internal interface IGateApi
    {
        [Get("/1/ticker/{currencyPair}")]
        Task<GateSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/1/tickers")]
        Task<Dictionary<string,GateSchema.TickerResponse>> GetTickersAsync();

        [Get("/1/pairs")]
        Task<string[]> GetAssetPairsAsync();

        [Get("/1/marketlist")]
        Task<GateSchema.VolumeResponse> GetVolumesAsync();

        [Get("/1/orderBook/{currencyPair}")]
        Task<GateSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
