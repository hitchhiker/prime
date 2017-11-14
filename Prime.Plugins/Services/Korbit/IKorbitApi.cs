using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Korbit
{
    internal interface IKorbitApi
    {
        [Get("/ticker?currency_pair={currencyPair}")]
        Task<KorbitSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker/detailed?currency_pair={currencyPair}")]
        Task<KorbitSchema.DetailedTickerResponse> GetDetailedTickerAsync([Path] string currencyPair);

        [Get("/orderbook?currency_pair={currencyPair}")]
        Task<KorbitSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
