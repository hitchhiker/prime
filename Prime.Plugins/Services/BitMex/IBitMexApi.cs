using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.BitMex
{
    [AllowAnyStatusCode]
    internal interface IBitMexApi
    {
        /// <summary>
        /// Gets deposit addresses for specific currency.
        /// </summary>
        /// <param name="currency">Currency which deposit address should be returned.</param>
        /// <returns>Deposit address for specified currency.</returns>
        [Get("/user/depositAddress")]
        Task<String> GetUserDepositAddressAsync([Query]String currency);

        /// <summary>
        /// Gets wallet information for specified currency.
        /// </summary>
        /// <param name="currency">Currency which wallet information should be returned.</param>
        /// <returns>Information about wallet.</returns>
        [Get("/user/wallet?currency={currency}")]
        Task<Response<BitMexSchema.WalletInfoResponse>> GetUserWalletInfoAsync([Path]String currency);

        /// <summary>
        /// Gets information about current user.
        /// </summary>
        /// <returns>The information about current user.</returns>
        [Get("/user")]
        Task<Response<BitMexSchema.UserInfoResponse>> GetUserInfoAsync();

        /// <summary>
        /// Gets user's orders.
        /// </summary>
        /// <param name="symbol">Instrument symbol.</param>
        /// <param name="filter">Generic table filter. Send JSON key/value pairs, such as {"key": "value"}. 
        /// You can key on individual fields, and do more advanced querying on timestamps. 
        /// See the "Timestamp Docs" for more details.</param>
        /// <param name="columns">Array of column names to fetch. If omitted, will return all columns.</param>
        /// <param name="count">Number of results to fetch.</param>
        /// <param name="start">Starting point for results.</param>
        /// <param name="reverse">If true, will sort results newest first.</param>
        /// <param name="startTime">Starting date filter for results.</param>
        /// <param name="endTime">Ending date filter for results.</param>
        /// <returns>List of orders.</returns>
        [Get("/order")]
        Task<Response<BitMexSchema.OrdersResponse>> GetOrdersAsync(
            [Query] string symbol = null, 
            [Query] string filter = null,
            [Query] string columns = null, 
            [Query] double? count = null, 
            [Query] double? start = null,
            [Query] bool? reverse = null, 
            [Query] DateTime? startTime = null, 
            [Query] DateTime? endTime = null);

        /// <summary>
        /// Places new order.
        /// </summary>
        /// <param name="ordType">Order type. Valid options: Market, Limit, Stop, StopLimit, MarketIfTouched, LimitIfTouched, MarketWithLeftOverAsLimit, Pegged. 
        /// Defaults to 'Limit' when price is specified. Defaults to 'Stop' when stopPx is specified. 
        /// Defaults to 'StopLimit' when price and stopPx are specified.</param>
        /// <param name="side">Order side. Valid options: Buy, Sell. Defaults to 'Buy' unless orderQty or simpleOrderQty is negative.</param>
        /// <param name="symbol">Instrument symbol. e.g. 'XBTUSD'.</param>
        /// <param name="orderQty">Order quantity in units of the instrument (i.e. contracts).</param>
        /// <param name="price">Optional limit price for 'Limit', 'StopLimit', and 'LimitIfTouched' orders.</param>
        /// <returns>Information about created order.</returns>
        [Post("/order")]
        Task<Response<BitMexSchema.NewOrderResponse>> CreateNewLimitOrderAsync([Query] string ordType, [Query] string side, [Query] string symbol, [Query] decimal orderQty, [Query] decimal price);

        /// <summary>
        /// Gets previous trades in time buckets.
        /// </summary>
        /// <param name="currencySymbol">Instrument symbol.</param>
        /// <param name="binSize">Time interval to bucket by. Available options: [1m,5m,1h,1d].</param>
        /// <param name="startTime">Starting date filter for results.</param>
        /// <param name="endTime">Ending date filter for results.</param>
        /// <returns>List if previous trades.</returns>
        [Get("/trade/bucketed?binSize={binSize}&partial=false&count=500&symbol={currencySymbol}&reverse=true&startTime={startTime}&endTime={endTime}")]
        Task<Response<BitMexSchema.BucketedTradeEntriesResponse>> GetTradeHistoryAsync([Path] string currencySymbol, [Path] string binSize, [Path(Format = "yyyy.MM.dd")] DateTime startTime, [Path(Format = "yyyy.MM.dd")] DateTime endTime);

        /// <summary>
        /// Gets all active instruments and instruments that have expired in less than 24hrs.
        /// </summary>
        /// <returns>List of available instruments.</returns>
        [Get("/instrument/active")]
        Task<Response<BitMexSchema.InstrumentsActiveResponse>> GetInstrumentsActiveAsync();

        /// <summary>
        /// Gets latest prices for specified asset pair or for all if asset pair is not specified.
        /// </summary>
        /// <param name="pairCode">Pair code which latest prices should be returned. If not set prices for all pairs will be returned.</param>
        /// <returns>Latest prices.</returns>
        [Get("/instrument?columns=[\"underlying\",\"quoteCurrency\",\"lastPrice\",\"highPrice\",\"lowPrice\",\"bidPrice\",\"askPrice\",\"timestamp\",\"symbol\",\"volume24h\"]&reverse=true&count=500&filter=%7B\"state\": \"Open\", \"typ\": \"FFWCSX\"%7D")]
        Task<Response<BitMexSchema.InstrumentLatestPricesResponse>> GetLatestPricesAsync([Query("symbol")] string pairCode = null);

        /// <summary>
        /// Gets order book for specified currency pair.
        /// </summary>
        /// <param name="symbol">Currency pair which order book should be returned.</param>
        /// <param name="depth">The number of order book records to be returned.</param>
        /// <returns>The list of order book records.</returns>
        [Get("/orderBook/L2")]
        Task<Response<BitMexSchema.OrderBookResponse>> GetOrderBookAsync([Query] string symbol, [Query] int depth);

        /// <summary>
        /// Get a history of all of your wallet transactions (deposits, withdrawals, PNL).
        /// </summary>
        /// <param name="currency">Currency which transactions should be returned.</param>
        /// <returns>List of wallet transactions.</returns>
        [Get("/user/walletHistory")]
        Task<Response<BitMexSchema.WalletHistoryResponse>> GetWalletHistoryAsync([Query] string currency);

        /// <summary>
        /// Requests withdrawal.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Information about requested withdrawal.</returns>
        [Post("/user/requestWithdrawal")]
        Task<Response<BitMexSchema.WithdrawalRequestResponse>> RequestWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        /// <summary>
        /// Cancels withdrawal.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Information about canceled withdrawal.</returns>
        [Post("/user/cancelWithdrawal")]
        Task<Response<BitMexSchema.WithdrawalCancelationResponse>> CancelWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)]Dictionary<string, object> body);

        /// <summary>
        /// Confirms withdrawal.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Information about confirmed withdrawal.</returns>
        [Post("/user/confirmWithdrawal")]
        Task<Response<BitMexSchema.WithdrawalConfirmationResponse>> ConfirmWithdrawalAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);

        /// <summary>
        /// Gets the number of users connected to chat.
        /// </summary>
        /// <returns>The number of users and bots connected to the chat.</returns>
        [Get("/chat/connected")]
        Task<Response<BitMexSchema.ConnectedUsersResponse>> GetConnectedUsersAsync();
    }
}
