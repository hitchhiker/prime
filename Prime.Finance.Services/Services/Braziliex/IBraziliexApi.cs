using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Finance.Services.Services.Dsx;
using RestEase;

namespace Prime.Finance.Services.Services.Braziliex
{
    [AllowAnyStatusCode]
    internal interface IBraziliexApi
    {
        [Get("/public/ticker/{currencyPair}")]
        Task<BraziliexSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<BraziliexSchema.AllTickersResponse> GetTickersAsync();

        [Get("/public/orderbook/{currencyPair}")]
        Task<BraziliexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/private")]
        Task<Response<BraziliexSchema.BalancesResponse>> GetBalancesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/private")]
        Task<Response<BraziliexSchema.NewOrderResponse>> PlaceLimitOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        [Post("/private")]
        Task<Response<BraziliexSchema.ActiveOrdersResponse>> QueryActiveOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
        
    }
}
