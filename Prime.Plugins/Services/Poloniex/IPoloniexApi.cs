using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Poloniex
{
    [AllowAnyStatusCode]
    internal interface IPoloniexApi
    {
        [Post("/tradingApi")]
        Task<Response<Dictionary<string, decimal>>> GetBalancesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.BalancesDetailedResponse>> GetBalancesDetailedAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.DepositAddressesResponse>> GetDepositAddressesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.OrderLimitResponse>> PlaceOrderLimitAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.OrderStatusResponse>> GetOrderTradesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.OpenMarketOrdersResponse>> GetOpenOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.MarketTradeOrdersResponse>> GetTradeHistoryAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.CancelOrderResponse>> CancelOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Get("/public?command=returnTicker")]
        Task<Response<PoloniexSchema.TickerResponse>> GetTickerAsync();

        [Get("/public?command=return24hVolume")]
        Task<Response<PoloniexSchema.VolumeResponse>> Get24HVolumeAsync();

        [Get("/public?command=returnChartData&currencyPair={currencyPair}&start={timeStampStart}&end={timeStampEnd}&period={period}")]
        Task<Response<PoloniexSchema.ChartEntriesResponse>> GetChartDataAsync([Path] string currencyPair, [Path] long timeStampStart, [Path] long timeStampEnd, [Path(Format = "D")] PoloniexTimeInterval period);

        [Get("/public?command=returnOrderBook&currencyPair={currencyPair}&depth={depth}")]
        Task<Response<PoloniexSchema.OrderBookResponse>> GetOrderBookAsync([Path] string currencyPair, [Path] int depth = 10000);
    }
}
