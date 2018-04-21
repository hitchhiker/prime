using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime.TimeZones;
using RestEase;

namespace Prime.Plugins.Services.HitBtc
{
    [AllowAnyStatusCode]
    internal interface IHitBtcApi
    {
        /// <summary>
        /// Gets symbols with their characteristics supported by exchange.
        /// See https://hitbtc.com/api#symbols.
        /// </summary>
        /// <returns>List of currency symbols with their characteristics.</returns>
        [Get("/public/symbol")]
        Task<Response<HitBtcSchema.SymbolsResponse>> GetSymbolsAsync();

        /// <summary>
        /// Gets deposit address for specified currency. If does not exist, it will be created.
        /// See https://hitbtc.com/api#getaddress.
        /// </summary>
        /// <param name="currency">Currency code which deposit address is to be returned.</param>
        /// <returns>Deposit address of specified currency.</returns>
        [Get("/account/crypto/address/{currency}")]
        Task<Response<HitBtcSchema.DepositAddressResponse>> GetDepositAddressAsync([Path] string currency);

        /// <summary>
        /// Gets payment balances.
        /// See https://hitbtc.com/api#paymentbalance.
        /// </summary>
        /// <returns>Multi-currency balance of the main account.</returns>
        [Get("/trading/balance")]
        Task<Response<HitBtcSchema.BalancesResponse>> GetTradingBalanceAsync();

        /// <summary>
        /// Gets information about placed order.
        /// </summary>
        /// <param name="clientOrderId">The id of order.</param>
        /// <param name="wait"></param>
        /// <returns>Information about the order with specified id.</returns>
        [Get("/order/{clientOrderId}")]
        Task<Response<HitBtcSchema.OrderInfoResponse>> GetActiveOrderInfoAsync([Path] string clientOrderId, [Query] int? wait = null);

        /// <summary>
        /// Places crypto withdrawal request.
        /// </summary>
        /// <param name="body">Post parameters.</param>
        /// <returns></returns>
        [Post("/account/crypto/withdraw")]
        Task<Response<HitBtcSchema.WithdrawCryptoResponse>> WithdrawCryptoAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        /// <summary>
        /// Creates new order.
        /// For more details see https://api.hitbtc.com/#create-new-order.
        /// </summary>
        /// <param name="body">Post parameters.</param>
        /// <returns>Information about new order.</returns>
        [Post("/order")]
        Task<Response<HitBtcSchema.OrderInfoResponse>> CreateNewOrderAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        /// <summary>
        /// Gets all orders. Orders older then 24 hours without trades are deleted.
        /// </summary>
        /// <param name="symbol">Optional parameter to filter active orders by symbol.</param>
        /// <param name="clientOrderId">If set, other parameters will be ignored. Without limit and pagination.</param>
        /// <param name="from">Start time.</param>
        /// <param name="till">End time.</param>
        /// <param name="number">Number of orders to be returned.</param>
        /// <param name="offset">Number of first orders to be skipped.</param>
        /// <returns>Orders history list.</returns>
        [Get("/history/order")]
        Task<Response<HitBtcSchema.OrderInfoResponses>> GetOrdersHistoryAsync([Query] string symbol = null, [Query] string clientOrderId = null, [Query] DateTime? from = null, [Query] DateTime? till = null, [Query] int? number = null, [Query] int? offset = null);

        /// <summary>
        /// Gets array of active orders.
        /// </summary>
        /// <param name="symbol">Optional parameter to filter active orders by symbol.</param>
        /// <returns>Open orders list.</returns>
        [Get("/order")]
        Task<Response<HitBtcSchema.OrderInfoResponses>> GetOpenOrdersAsync([Query] string symbol = null);

        /// <summary>
        /// Gets tickers for all currencies.
        /// See https://hitbtc.com/api#alltickers.
        /// </summary>
        /// <returns>Associative array of pair code and ticker data</returns>
        [Get("/public/ticker")]
        Task<Response<HitBtcSchema.TickersResponse>> GetAllTickersAsync();

        /// <summary>
        /// Get ticker for specified currency pair.
        /// See https://hitbtc.com/api#ticker.
        /// </summary>
        /// <param name="pairCode">Currency which ticker is to be returned.</param>
        /// <returns>Ticker data.</returns>
        [Get("/public/ticker/{pairCode}")]
        Task<Response<HitBtcSchema.TickerResponse>> GetTickerAsync([Path] string pairCode);
    }
}