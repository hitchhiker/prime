using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Gate
{
    //[AllowAnyStatusCode]
    internal interface IGateApi
    {
        [Get("/1/ticker/{currencyPair}")]
        Task<GateSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/1/tickers")]
        Task<Dictionary<string, GateSchema.TickerResponse>> GetTickersAsync();

        [Get("/1/pairs")]
        Task<string[]> GetAssetPairsAsync();

        [Get("/1/marketlist")]
        Task<GateSchema.VolumeResponse> GetVolumesAsync();

        [Get("/1/orderBook/{currencyPair}")]
        Task<GateSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/1/private/balances")]
        Task<Response<GateSchema.BalancesResponse>> GetBalancesAsync();

        [Post("/1/private/{buySell}")]
        Task<Response<GateSchema.NewOrderResponse>> NewOrderAsync([Path] string buySell, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/1/private/getOrder")]
        Task<Response<GateSchema.OrderResponse>> QueryOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/1/private/withdraw")]
        Task<Response<GateSchema.WithdrawalResponse>> PlaceWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
