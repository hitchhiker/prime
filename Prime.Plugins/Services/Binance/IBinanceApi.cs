using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Binance
{
    [AllowAnyStatusCode]
    internal interface IBinanceApi
    {
        /// <summary>
        /// For more details see https://www.binance.com/restapipub.html#user-content-market-data-endpoints.
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/ticker/allPrices")]
        Task<Response<BinanceSchema.LatestPricesResponse>> GetSymbolPriceTickerAsync();

        /// <summary>
        /// Maximum number of order book records is 100.
        /// </summary>
        /// <param name="symbol">Currency pair code.</param>
        /// <param name="limit">The maximum number or records to return.</param>
        /// <returns>List of order book entries.</returns>
        [Get("/api/v1/depth?symbol={symbol}&limit={limit}")]
        Task<Response<BinanceSchema.OrderBookResponse>> GetOrderBookAsync([Path] string symbol, [Path] int limit = 100);

        /// <summary>
        /// Gets user account information.
        /// For more details see https://www.binance.com/restapipub.html#user-content-account-endpoints.
        /// </summary>
        /// <returns>Information about user account.</returns>
        [Get("/api/v3/account")]
        Task<Response<BinanceSchema.UserInformationResponse>> GetAccountInformationAsync();

        /// <summary>
        /// Places a new trade limit order.
        /// </summary>
        /// <returns>The id of placed order.</returns>
        /// <param name="symbol">The market.</param>
        /// <param name="side">The type of order, can be "buy" or "sell".</param>
        /// <param name="type">The type of trade order.</param>
        /// <param name="timeInForce">The method of order execution, can be "GTC" (Good 'til Canceled) or "IOC" (Immediate or Cancel).</param>
        /// <param name="quantity">The volume of order.</param>
        /// <param name="price">The price of order.</param>
        /// <param name="newClientOrderId">New client order identifier.</param>
        /// <param name="stopPrice">Stop price.</param>
        /// <param name="icebergQty">Iceberg quantity.</param>
        /// <param name="recvWindow">Request receive window.</param>
        [Post("/api/v3/order")]
        Task<Response<BinanceSchema.NewOrderResponse>> NewOrderAsync([Query] string symbol, [Query] string side, [Query] string type, [Query] string timeInForce, [Query] decimal quantity, [Query] decimal price, [Query] string newClientOrderId = null, [Query] decimal? stopPrice = null, [Query] decimal? icebergQty = null, [Query] long? recvWindow = null);

        /// <summary>
        /// Gets the information about placed order by its id (should be specified either <param name="orderId">orderId</param> or <param name="origClientOrderId">origClientOrderId</param>).
        /// </summary>
        /// <param name="symbol">The market where the order was placed.</param>
        /// <param name="orderId">The numeric id of order.</param>
        /// <param name="origClientOrderId">The string id of order.</param>
        /// <param name="recvWindow">Request receive window.</param>
        /// <returns></returns>
        [Get("/api/v3/order")]
        Task<Response<BinanceSchema.QueryOrderResponse>> QueryOrderAsync([Query] string symbol, [Query] long? orderId = null, [Query] string origClientOrderId = null, [Query] long? recvWindow = null);

        /// <summary>
        /// Gets the history of deposits to account.
        /// </summary>
        /// <param name="asset">Asset which deposit history should be returned.</param>
        /// <param name="status">Status filter.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        /// <param name="recvWindow">Request receive window.</param>
        /// <returns></returns>
        [Get("/wapi/v3/depositHistory.html")]
        Task<Response<BinanceSchema.DepositHistoryResponse>> GetDepositHistoryAsync([Query] string asset = null, [Query] int? status = null, [Query] long? startTime = null, [Query] long? endTime = null, [Query] long? recvWindow = null);

        /// <summary>
        /// Submits withdrawal request.
        /// </summary>
        /// <param name="asset">Withdrawal asset.</param>
        /// <param name="address">Address which money is to be withdrawn to.</param>
        /// <param name="amount">Amount of money to be withdrawn.</param>
        /// <param name="addressTag">Optional address tag. Required for XRP, XMR and other.</param>
        /// <param name="name">The name of the address.</param>
        /// <param name="recvWindow">Request receive window.</param>
        /// <returns></returns>
        [Post("/wapi/v3/withdraw.html")]
        Task<Response<BinanceSchema.WithdrawalRequestResponse>> SubmitWithdrawRequestAsync([Query] string asset, [Query] string address, [Query] decimal amount, [Query] string addressTag = null, [Query] string name = null, [Query] long? recvWindow = null);

        /// <summary>
        /// Gets OHLC data.
        /// </summary>
        /// <param name="currency">Currency pair code being queried.</param>
        /// <param name="interval">Time interval.</param>
        /// <param name="startTime">Start time.</param>
        /// <param name="endTime">End time.</param>
        /// <param name="limit">Limits the number of records to be returned.</param>
        /// <returns>Array of decimals that represent OHLC record.</returns>
        [Get("/api/v1/klines?symbol={currency}&interval={interval}")]
        Task<Response<BinanceSchema.CandlestickResponse>> GetCandlestickBarsAsync([Path] string currency, [Path] string interval, [Query] long? startTime = null, [Query] long? endTime = null, [Query] int? limit = null);

        /// <summary>
        /// Gets 24-hr ticker.
        /// </summary>
        /// <param name="currency">Currency which ticker is to be returned.</param>
        /// <returns>24 hour ticker.</returns>
        [Get("/api/v1/ticker/24hr?symbol={currency}")]
        Task<Response<BinanceSchema.Ticker24HrResponse>> Get24HrTickerAsync([Path] string currency);

        /// <summary>
        /// Checks server availability.
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/ping")]
        Task<Response<object>> PingAsync();
    }
}
