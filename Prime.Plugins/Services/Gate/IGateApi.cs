using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Gate
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
    }
}
