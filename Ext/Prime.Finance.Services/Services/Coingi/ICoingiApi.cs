using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coingi
{
    internal interface ICoingiApi
    {
        [Get("/order-book/{currencyPair}/{maxAskCount}/{maxBidCount}/{maxDepthRangeCount}")]
        Task<CoingiSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int maxAskCount, [Path] int maxBidCount, [Path] int maxDepthRangeCount);

        [Get("/transactions/{currencyPair}/{maxCount}")]
        Task<CoingiSchema.TransactionResponse[]> GetTransactionListAsync([Path] string currencyPair, [Path] int maxCount);
    }
}
