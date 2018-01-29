using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Plugins.Services.NovaExchange;
using RestEase;

namespace Prime.Plugins.Services.Liqui
{
    internal interface ILiquiApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<LiquiSchema.AllTickersResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/ticker/{currencyPair}")]
        Task<LiquiSchema.AllTickersResponse> GetTickersAsync([Path] string currencyPair);

        [Get("/info")]
        Task<LiquiSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/depth/{currencyPair}")]
        Task<LiquiSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);
    }
}
