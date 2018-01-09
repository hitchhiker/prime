using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coinmate
{
    internal interface ICoinmateApi
    {
        [Get("/ticker?currencyPair={currencyPair}")]
        Task<CoinmateSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/orderBook?currencyPair={currencyPair}&groupByPriceLimit={groupByPriceLimit}")]
        Task<CoinmateSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] bool groupByPriceLimit);
    }
}
