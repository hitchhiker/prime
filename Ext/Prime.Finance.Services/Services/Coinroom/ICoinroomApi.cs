using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinroom
{
    [AllowAnyStatusCode]
    internal interface ICoinroomApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<CoinroomSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/availableCurrencies")]
        Task<CoinroomSchema.CurrenciesResponse> GetCurrenciesAsync();

        [Post("/orderbook")]
        Task<CoinroomSchema.OrderBookResponse> GetOrderBookAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/balances")]
        Task<Response<CoinroomSchema.BalancesResponse>> GetBalancesAsync();

        //TODO - SC - Confirm response format since it is not documented in API doc
        [Post("/placeOrder")]
        Task<Response<object>> NewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
        
        [Post("/orders")]
        Task<Response<CoinroomSchema.OrdersResponse>> QueryOrdersAsync();

        //TODO - SC - Confirm response format since it is not documented in API doc
        [Post("/payoutCrypto")]
        Task<Response<object>> PlaceWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
