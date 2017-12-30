using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coincheck
{
    internal interface ICoincheckApi
    {
        [Get("/ticker/?pair={currencyPair}")]
        Task<CoincheckSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/order_books")]
        Task<CoincheckSchema.OrderBookResponse> GetOrderBookAsync();
    }
}
