using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Dsx
{
    [AllowAnyStatusCode]
    internal interface IDsxApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<DsxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/info")]
        Task<DsxSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/depth/{currencyPair}")]
        Task<DsxSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/info/account")]
        Task<Response<DsxSchema.BalanceResponse>> GetBalancesAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);

        [Post("/order/new")]
        Task<Response<DsxSchema.OrderResponse>> NewOrderAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);

        [Post("/order/status")]
        Task<Response<DsxSchema.OrderStatusResponse>> QueryOrderAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);

        [Post("/withdraw/crypto")]
        Task<Response<DsxSchema.WithdrawalResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);
    }
}
