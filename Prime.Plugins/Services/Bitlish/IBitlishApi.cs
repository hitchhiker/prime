using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitlish
{
    internal interface IBitlishApi
    {
        [Get("/tickers")]
        Task<BitlishSchema.AllTickersResponse> GetTickersAsync();

        [Get("/trades_depth?pair_id={currencyPair}")]
        Task<BitlishSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
