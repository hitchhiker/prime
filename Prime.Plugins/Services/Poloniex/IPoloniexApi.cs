using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Poloniex
{
    [AllowAnyStatusCode]
    internal interface IPoloniexApi
    {
        /// <summary>
        /// Returns all of your available balance.
        /// </summary>
        /// <param name="data">Post body. Method "returnBalances".</param>
        /// <returns>Balances information.</returns>
        [Post("/tradingApi")]
        Task<Response<Dictionary<string, decimal>>> GetBalancesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Returns all of your balances, including available balance, balance on orders, and the estimated BTC value of your balance.
        /// By default, this call is limited to your exchange account; set the "account" POST parameter to "all" to include your margin and lending accounts.
        /// </summary>
        /// <param name="data">Post body. Method "returnCompleteBalances".</param>
        /// <returns>Detailed balances information.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.BalancesDetailedResponse>> GetBalancesDetailedAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Returns all of your deposit addresses.
        /// </summary>
        /// <param name="data">Post body. Method "returnDepositAddresses".</param>
        /// <returns>Deposit addresses.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.DepositAddressesResponse>> GetDepositAddressesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Places a limit buy order in a given market. Required POST parameters are "currencyPair", "rate", and "amount". If successful, the method will return the order number. 
        /// </summary>
        /// <param name="data">Post body. Method "buy" or "sell".</param>
        /// <returns>Placed order information.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.OrderLimitResponse>> PlaceOrderLimitAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Returns all trades involving a given order, specified by the "orderNumber" POST parameter. 
        /// If no trades for the order have occurred or you specify an order that does not belong to you, you will receive an error.
        /// </summary>
        /// <param name="data">Post body. Method "returnOrderTrades".</param>
        /// <returns>Order trades list.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.TradesResponse>> GetOrderTradesAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Returns your open orders for a given market, specified by the "currencyPair" POST parameter, e.g. "BTC_XCP". 
        /// Set "currencyPair" to "all" to return open orders for all markets.
        /// </summary>
        /// <param name="data">Post body. Method "returnOpenOrders".</param>
        /// <returns>List of open orders.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.OpenMarketOrdersResponse>> GetOpenOrdersAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Returns your trade history for a given market, specified by the "currencyPair" POST parameter. 
        /// You may specify "all" as the currencyPair to receive your trade history for all markets. 
        /// You may optionally specify a range via "start" and/or "end" POST parameters, given in UNIX timestamp format; if you do not specify a range, it will be limited to one day.
        /// You may optionally limit the number of entries returned using the "limit" parameter, up to a maximum of 10,000. If the "limit" parameter is not specified, no more than 500 entries will be returned.
        /// </summary>
        /// <param name="data">Post body. Method "returnTradeHistory".</param>
        /// <returns>Orders history.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.MarketTradeOrdersResponse>> GetTradeHistoryAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Immediately places a withdrawal for a given currency, with no email confirmation. 
        /// In order to use this method, the withdrawal privilege must be enabled for your API key. 
        /// Required POST parameters are "currency", "amount", and "address". 
        /// For XMR withdrawals, you may optionally specify "paymentId".
        /// </summary>
        /// <param name="data">Post body. Method "withdraw".</param>
        /// <returns>Withdrawal result represented by sentence in free form.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.WithdrawalResponse>> WithdrawAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Cancels an order you have placed in a given market. Required POST parameter is "orderNumber".
        /// </summary>
        /// <param name="data">Post body. Method "cancelOrder".</param>
        /// <returns>Operation status.</returns>
        [Post("/tradingApi")]
        Task<Response<PoloniexSchema.CancelOrderResponse>> CancelOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        /// <summary>
        /// Returns the ticker for all markets.
        /// </summary>
        /// <returns>Ticker for all markets.</returns>
        [Get("/public?command=returnTicker")]
        Task<Response<PoloniexSchema.TickerResponse>> GetTickerAsync();

        /// <summary>
        /// Returns the 24-hour volume for all markets, plus totals for primary currencies.
        /// </summary>
        /// <returns>24h volume information for all markets</returns>
        [Get("/public?command=return24hVolume")]
        Task<Response<PoloniexSchema.VolumeResponse>> Get24HVolumeAsync();

        /// <summary>
        /// Returns candlestick chart data (OHLC data). 
        /// Required GET parameters are "currencyPair", "period" (candlestick period in seconds; valid values are 300, 900, 1800, 7200, 14400, and 86400), 
        /// "start", and "end". "Start" and "end" are given in UNIX timestamp format and used to specify the date range for the data returned.
        /// </summary>
        /// <param name="currencyPair">Target market.</param>
        /// <param name="timeStampStart">Start time UNIX timestamp.</param>
        /// <param name="timeStampEnd">End time UNIX timestamp.</param>
        /// <param name="period">The period in seconds.</param>
        /// <returns>OHLC data.</returns>
        [Get("/public?command=returnChartData")]
        Task<Response<PoloniexSchema.ChartEntriesResponse>> GetChartDataAsync([Query] string currencyPair, [Query("start")] long timeStampStart, [Query("end")] long timeStampEnd, [Query(Format = "D")] PoloniexTimeInterval period);

        /// <summary>
        /// Returns the order book for a given market, as well as a sequence number for use with the Push API and an indicator specifying whether the market is frozen. 
        /// You may set currencyPair to "all" to get the order books of all markets.
        /// </summary>
        /// <param name="currencyPair">Target market.</param>
        /// <param name="depth">Number of order book records.</param>
        /// <returns>Order book data.</returns>
        [Get("/public?command=returnOrderBook")]
        Task<Response<PoloniexSchema.OrderBookResponse>> GetOrderBookAsync([Query] string currencyPair, [Query()] int depth = 10000);
    }
}
