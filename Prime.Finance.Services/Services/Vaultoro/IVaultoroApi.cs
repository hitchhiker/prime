using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Vaultoro
{
    [AllowAnyStatusCode]
    internal interface IVaultoroApi
    {
        [Get("/markets")]
        Task<VaultoroSchema.MarketResponse> GetMarketsAsync();

        [Get("/orderbook")]
        Task<VaultoroSchema.OrderBookResponse> GetOrderBookAsync();

        [Get("/balance")]
        Task<Response<VaultoroSchema.BalanceResponse>> GetBalanceAsync();

        [Post("/{type}/gld/limit?{asset}={amount}&price={price}")]
        Task<Response<VaultoroSchema.OrderResponse>> NewOrderAsync([Path] string type, [Path] string asset, [Path] decimal amount, [Path] decimal price);

        [Get("/orders")]
        Task<Response<VaultoroSchema.OrdersResponse>> QueryOrdersAsync();

        [Post("/withdraw?btc={amount}")]
        Task<Response<VaultoroSchema.WithdrawalResponse>> SubmitWithdrawRequestAsync([Path] decimal amount);
    }
}
