using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Poloniex
{
    internal interface IPoloniexApi
    {
        [Post("/tradingApi")]
        Task<Dictionary<string, decimal>> GetBalancesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<PoloniexSchema.BalancesDetailedResponse> GetBalancesDetailedAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<PoloniexSchema.DepositAddressesResponse> GetDepositAddressesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Get("/public?command=returnTicker")]
        Task<PoloniexSchema.TickerResponse> GetTickerAsync();

        [Get("/public?command=return24hVolume")]
        Task<PoloniexSchema.VolumeResponse> Get24HVolumeAsync();

        [Get("/public?command=returnChartData&currencyPair={currencyPair}&start={timeStampStart}&end={timeStampEnd}&period={period}")]
        Task<PoloniexSchema.ChartEntriesResponse> GetChartDataAsync([Path] string currencyPair, [Path] long timeStampStart, [Path] long timeStampEnd, [Path(Format = "D")] PoloniexTimeInterval period);

        [Get("/public?command=returnOrderBook&currencyPair={currencyPair}&depth={depth}")]
        Task<PoloniexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair, [Path] int depth = 10000);
    }
}
