using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.SouthXchange
{
    internal interface ISouthXchangeApi
    {
        [Get("/price/{currencyPair}")]
        Task<SouthXchangeSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/prices")]
        Task<SouthXchangeSchema.AllTickerResponse[]> GetTickersAsync();

        [Get("/book/{currencyPair}")]
        Task<SouthXchangeSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
