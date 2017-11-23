using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BtcMarkets
{
    internal interface IBtcMarketsApi
    {
        [Get("/market/{currencyPair}/tick")]
        Task<BtcMarketsSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);
    }
}
