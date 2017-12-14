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
        Task<CryptopiaSchema.TickerResponse> GetTickerAsync([Path] string assetPair);

        [Get("/GetMarkets")]
        Task<CryptopiaSchema.AllTickersResponse> GetTickersAsync();

        [AllowAnyStatusCode]
        [Post("/GetBalance")]
        Task<Response<CryptopiaSchema.BalanceResponse>> GetBalanceAsync([Body(BodySerializationMethod.Serialized)] object body);
    }
}
