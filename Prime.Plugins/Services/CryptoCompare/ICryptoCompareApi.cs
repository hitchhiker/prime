using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestEase;

namespace Prime.Plugins.Services.CryptoCompare
{
    internal interface ICryptoCompareApi
    {
        [Get("coinlist")]
        Task<CryptoCompareSchema.CoinListResult> GetCoinListAsync();

        [Get("pricemulti")]
        Task<string> GetPricesAsync(string fsyms, string tsyms, string e, string extraParams, string sign, string tryConversion);
        
        [Get("coinsnapshot")]
        Task<CryptoCompareSchema.CoinSnapshotResult> GetCoinSnapshotAsync(string fsym, string tsym);

        [Get("top/exchanges")]
        Task<CryptoCompareSchema.TopExchangesResult> GetTopExchangesAsync(string fsym, string tsym);

        [Get("all/exchanges")]
        Task<CryptoCompareSchema.AssetPairsAllExchanges> GetAssetPairsAllExchangesAsync();

        [Get("histohour")]
        Task<CryptoCompareSchema.HistoricListResult> GetHistoricalHourlyAsync(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs);

        [Get("histominute")]
        Task<CryptoCompareSchema.HistoricListResult> GetHistoricalMinuteAsync(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs);

        [Get("histoday")]
        Task<CryptoCompareSchema.HistoricListResult> GetHistoricalDayAsync(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs, string allData);
    }
}