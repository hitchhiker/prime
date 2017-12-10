using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitfinex
{
    internal interface IBitfinexApi
    {
        [Get("/pubticker/{currencyPair}")]
        Task<BitfinexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/symbols")]
        Task<string[]> GetAssetsAsync();

        [Post("/account_infos")]
        Task<BitfinexSchema.AccountInfoResponse> GetAccountInfoAsync([Body(BodySerializationMethod.Serialized)] object body);
    }
}
