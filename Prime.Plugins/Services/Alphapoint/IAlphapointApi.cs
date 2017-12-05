using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Alphapoint
{
    internal interface IAlphapointApi
    {
        [Post("/GetTicker")]
        Task<AlphapointSchema.TickerResponse> GetTickerAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);

        [Post("/GetProductPairs")]
        Task<AlphapointSchema.ProductPairsResponse> GetAssetPairsAsync();
    }
}
