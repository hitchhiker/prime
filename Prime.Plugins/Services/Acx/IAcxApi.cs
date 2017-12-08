using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Acx
{
    internal interface IAcxApi
    {
        [Get("/tickers/{currencyPair}.json")]
        Task<AcxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/tickers.json")]
        Task<AcxSchema.AllTickersResponse> GetTickersAsync();

        [Get("/markets.json")]
        Task<AcxSchema.MarketResponse[]> GetAssetPairsAsync();
    }
}
