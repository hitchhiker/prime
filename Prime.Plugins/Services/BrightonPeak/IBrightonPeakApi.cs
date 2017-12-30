using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BrightonPeak
{
    internal interface IBrightonPeakApi
    {
        [Post("/GetTicker")]
        Task<BrightonPeakSchema.TickerResponse> GetTickerAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);

        [Post("/GetProductPairs")]
        Task<BrightonPeakSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/GetOrderBook")]
        Task<BrightonPeakSchema.OrderBookResponse> GetOrderBookAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);
    }
}
