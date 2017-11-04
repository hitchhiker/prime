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

        [Get("all/exchanges")]
        Task<CryptoCompareSchema.AssetPairsAllExchanges> GetAssetPairsAllExchanges();

        [Get("histohour")]
        Task<CryptoCompareSchema.HistoricListResult> GetHistoricalHourly(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs);

        [Get("histominute")]
        Task<CryptoCompareSchema.HistoricListResult> GetHistoricalMinute(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs);

        [Get("histoday")]
        Task<CryptoCompareSchema.HistoricListResult> GetHistoricalDay(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs, string allData);
    }
}