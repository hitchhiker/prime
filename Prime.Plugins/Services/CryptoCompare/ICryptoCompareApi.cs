using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Core;
using plugins.Services.CryptoCompare.Model;
using RestEase;

namespace plugins
{
    public interface ICryptoCompareApi
    {
        [Get("coinlist")]
        Task<CoinListResult> GetCoinListAsync();

        [Get("price")]
        Task<Dictionary<string,double>> GetPricesAsync(string fsym, string tsyms, string e, string extraParams, string sign, string tryConversion);
        
        [Get("coinsnapshot")]
        Task<CoinSnapshotResult> GetCoinSnapshotAsync(string fsym, string tsym);

        [Get("histohour")]
        Task<HistoricListResult> GetHistoricalHourly(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs);

        [Get("histominute")]
        Task<HistoricListResult> GetHistoricalMinute(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs);

        [Get("histoday")]
        Task<HistoricListResult> GetHistoricalDay(string fsym, string tsym, string e, string extraParams, string sign, string tryConversion, int aggregate, int limit, long toTs, string allData);
    }
}