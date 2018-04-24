using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Dsx
{
    internal interface IDsxApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<DsxSchema.TickerResponse> GetTickerAsync([Path] string currencyPair);

        [Get("/info")]
        Task<DsxSchema.AssetPairsResponse> GetAssetPairsAsync();

        [Get("/depth/{currencyPair}")]
        Task<DsxSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Post("/info/account")]
        Task<DsxSchema.BalanceResponse> GetBalancesAsync([Body(BodySerializationMethod.Default)] Dictionary<string, object> body);
    }
}
