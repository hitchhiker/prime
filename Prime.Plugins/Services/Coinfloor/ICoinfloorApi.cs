using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Coinfloor
{
    internal interface ICoinfloorApi
    {
        [Get("/{currencyPair}/ticker/")]
        Task<CoinfloorSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);
    }
}
