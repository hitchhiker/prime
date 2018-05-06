using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Luno
{
    [AllowAnyStatusCode]
    internal interface ILunoApi
    {
        [Get("/ticker?pair={currencyPair}")]
        Task<LunoSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers")]
        Task<LunoSchema.AllTickersResponse> GetTickersAsync();

        [Get("/orderbook?pair={currencyPair}")]
        Task<LunoSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/balance")]
        Task<Response<LunoSchema.BalancesResponse>> GetBalancesAsync();

        [Post("/postorder")]
        Task<Response<LunoSchema.OrderResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Get("/orders/{id}")]
        Task<Response<LunoSchema.OrderResponse>> QueryOrderAsync([Path] string id);

        [Post("/send")]
        Task<Response<LunoSchema.WithdrawalResponse>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);
        
    }
}
