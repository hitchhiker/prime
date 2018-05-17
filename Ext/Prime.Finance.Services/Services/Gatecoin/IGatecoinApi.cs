using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Gatecoin
{
    internal interface IGatecoinApi
    {
        [Get("/Public/LiveTicker/{currencyPair}")]
        Task<GatecoinSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/Public/LiveTickers")]
        Task<GatecoinSchema.TickersResponse> GetTickersAsync();

        [Get("/Public/MarketDepth/{currencyPair}")]
        Task<GatecoinSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/Balance/Balances")]
        Task<Response<GatecoinSchema.BalancesResponse>> GetBalancesAsync();

        [Get("/Trade/Orders/{orderId}")]
        Task<Response<GatecoinSchema.OrderResponse>> QueryOrderAsync([Path] string orderId);

        [Post("/Trade/Orders")]
        Task<Response<GatecoinSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/ElectronicWallet/Withdrawals/{currency}")]
        Task<Response<GatecoinSchema.WithdrawalResponse>> PlaceWithdrawalAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body, [Path] string currency);
    }
}
