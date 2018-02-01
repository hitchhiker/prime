using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitBay
{
    internal interface IBitBayApi
    {
        [Get("/Public/{currencyPair}/ticker.json")]
        Task<BitBaySchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/Public/{currencyPair}/orderbook.json")]
        Task<BitBaySchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/Trading/tradingApi.php")]
        Task<object> GetUserInfoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
