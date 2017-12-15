using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Kuna
{
    internal interface IKunaApi
    {
        [Get("/tickers/{currencyPair}")]
        Task<KunaSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers")]
        Task<KunaSchema.AllTickersResponse> GetTickersAsync();

        [Get("/timestamp")]
        Task<long> GetTimestamp();
    }
}
