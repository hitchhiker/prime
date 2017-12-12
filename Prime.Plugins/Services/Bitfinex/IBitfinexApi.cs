using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitfinex
{
    [AllowAnyStatusCode]
    internal interface IBitfinexApi
    {
        [Get("/pubticker/{currencyPair}")]
        Task<Response<BitfinexSchema.TickerResponse>> GetTickerAsync([Path] string currencyPair);

        [Get("/symbols")]
        Task<Response<string[]>> GetAssetsAsync();

        [Post("/account_infos")]
        Task<Response<BitfinexSchema.AccountInfoResponse>> GetAccountInfoAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/balances")]
        Task<Response<BitfinexSchema.WalletBalanceResponse>> GetWalletBalancesAsync([Body(BodySerializationMethod.Serialized)] object body);

        [Post("/order/new")]
        Task<Response<BitfinexSchema.NewOrderResponse>> PlaceNewOrderAsync([Body(BodySerializationMethod.Serialized)] object body);
    }
}
