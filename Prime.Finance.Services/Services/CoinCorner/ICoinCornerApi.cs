using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.CoinCorner
{
    [AllowAnyStatusCode]
    internal interface ICoinCornerApi
    {
        [Get("/Ticker?Coin={Coin}&Currency={Currency}")]
        Task<CoinCornerSchema.TickerResponse> GetTickerAsync([Path] string Coin, [Path] string Currency);

        [Get("/OrderBook?Coin={Coin}")]
        Task<CoinCornerSchema.OrderBookResponse> GetOrderBookAsync([Path] string Coin);

        [Post("/AccountBalance")]
        Task<Response<CoinCornerSchema.BalancesResponse>> GetBalancesAsync();

        //TODO - SC: Documentation does not supply any response information
        [Post("/{buyOrSell}")]
        Task<Response<object>> NewOrderAsync([Path] string buyOrSell, [Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/OpenOrders")]
        Task<Response<CoinCornerSchema.OrderResponse[]>> QueryActiveOrdersAsync();

        //TODO - SC: Documentation does not supply any response information
        [Post("/WithdrawCoins")]
        Task<Response<object>> SubmitWithdrawRequestAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

    }
}
