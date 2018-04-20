using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bitfinex
{
    [AllowAnyStatusCode]
    internal interface IBitfinexApi
    {
        /// <summary>
        /// The ticker is a high level overview of the state of the market. 
        /// It shows the current best bid and ask, as well as the last trade price. 
        /// It also includes information such as daily volume and how much the price has moved over the last day.
        /// </summary>
        /// <param name="symbol">The symbol you want information about.</param>
        /// <returns>Ticker for the specified symbol.</returns>
        [Get("/pubticker/{symbol}")]
        Task<Response<BitfinexSchema.TickerResponse>> GetTickerAsync([Path] string symbol);

        /// <summary>
        /// Gets the list of symbol names.
        /// </summary>
        /// <returns>List of symbol names.</returns>
        [Get("/symbols")]
        Task<Response<string[]>> GetAssetsAsync();

        /// <summary>
        /// Gets the full order book.
        /// </summary>
        /// <param name="symbol">The symbol which order book should be returned.</param>
        /// <returns>Order book for specified symbol.</returns>
        [Get("/book/{symbol}")]
        Task<Response<BitfinexSchema.OrderBookResponse>> GetOrderBookAsync([Path] string symbol);

        /// <summary>
        /// Return information about your account (trading fees).
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Account information.</returns>
        [Post("/account_infos")]
        Task<Response<BitfinexSchema.AccountInfoResponse>> GetAccountInfoAsync([Body(BodySerializationMethod.Serialized)] object body);

        /// <summary>
        /// Returns account balances.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Account balances information.</returns>
        [Post("/balances")]
        Task<Response<BitfinexSchema.WalletBalanceResponse>> GetWalletBalancesAsync([Body(BodySerializationMethod.Serialized)] object body);

        /// <summary>
        /// Submit a new Order.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Information about placed order.</returns>
        [Post("/order/new")]
        Task<Response<BitfinexSchema.NewOrderResponse>> PlaceNewOrderAsync([Body(BodySerializationMethod.Serialized)] object body);

        /// <summary>
        /// Request a withdrawal from one of your wallet.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Withdrawal id and status.</returns>
        [Post("/withdraw")]
        Task<Response<BitfinexSchema.WithdrawalsResponse>> WithdrawAsync([Body(BodySerializationMethod.Serialized)] object body);

        /// <summary>
        /// Get the status of an order.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>The status of the order with given Id.</returns>
        [Post("/order/status")]
        Task<Response<BitfinexSchema.OrderStatusResponse>> GetOrderStatusAsync([Body(BodySerializationMethod.Serialized)] object body);

        /// <summary>
        /// Get active orders.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>The list of open orders.</returns>
        [Post("/orders")]
        Task<Response<BitfinexSchema.ActiveOrdersResponse>> GetActiveOrdersAsync([Body(BodySerializationMethod.Serialized)] object body);

        /// <summary>
        /// Get latest inactive orders. Limited to last 3 days and 1 request per minute.
        /// </summary>
        /// <param name="body">Post body.</param>
        /// <returns>Orders history.</returns>
        [Post("/orders/hist")]
        Task<Response<BitfinexSchema.OrdersHistoryResponse>> GetOrdersHistoryAsync([Body(BodySerializationMethod.Serialized)] object body);
    }
}
