using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coingi
{
    [AllowAnyStatusCode]
    internal interface ICoingiApi
    {
        [Get("/current/order-book/{currencyPair}/{maxAskCount}/{maxBidCount}/{maxDepthRangeCount}")]
        Task<CoingiSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int maxAskCount, [Path] int maxBidCount, [Path] int maxDepthRangeCount);

        [Get("/current/transactions/{currencyPair}/{maxCount}")]
        Task<CoingiSchema.TransactionResponse[]> GetTransactionListAsync([Path] string currencyPair, [Path] int maxCount);

        [Post("/user/balance")]
        Task<Response<CoingiSchema.BalanceResponse[]>> GetBalancesAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);

        [Post("/user/add-order")]
        Task<Response<CoingiSchema.NewOrderResponse>> NewOrderAsync([Body(BodySerializationMethod.Serialized)] Dictionary<string, object> body);
    }
}
