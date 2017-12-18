using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Xbtce
{
    [Header("Content-Type", "application/json")]
    [Header("Accept", "application/json")]
    [Header("Accept-Encoding", "gzip")]
    internal interface IXbtceApi
    {
        [Get("/public/ticker/{currencyPair}")]
        Task<XbtceSchema.TickerResponse[]> GetTickerAsync([Path] string currencyPair);

        [Get("/public/ticker")]
        Task<XbtceSchema.TickerResponse[]> GetTickersAsync();
    }
}
