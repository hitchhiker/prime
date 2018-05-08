using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Bittrex
{
    [AllowAnyStatusCode]
    internal interface IBittrexApi
    {
        /// <summary>
        /// Used to retrieve all balances from your account.
        /// </summary>
        /// <returns>Balance response data.</returns>
        [Get("/account/getbalances")]
        Task<Response<BittrexSchema.BalancesResponse>> GetBalancesAsync();

        /// <summary>
        /// Gets or generates new deposit address for specified currency.
        /// </summary>
        /// <param name="currency">Currency which deposit addresses should be returned (i.g. BTC, LTC).</param>
        /// <returns>Deposit addresses for specified currency.</returns>
        [Get("/account/getdepositaddress?currency={currency}")]
        Task<BittrexSchema.DepositAddressResponse> GetDepositAddressAsync([Path] string currency);

        /// <summary>
        /// Used to get the open and available trading markets at Bittrex along with other meta data.
        /// </summary>
        /// <returns>Markets information.</returns>
        [Get("/public/getmarkets")]
        Task<Response<BittrexSchema.MarketEntriesResponse>> GetMarketsAsync();

        /// <summary>
        /// Used to get the current tick values for a market.
        /// </summary>
        /// <param name="market">The market which ticker should be returned.</param>
        /// <returns>Current ticker for specified market</returns>
        [Get("/public/getticker?market={market}")]
        Task<BittrexSchema.TickerResponse> GetTickerAsync([Path] string market);

        /// <summary>
        /// Used to get the last 24 hour summary of all active markets.
        /// </summary>
        /// <returns>Market summary for each supported market.</returns>
        [Get("/public/getmarketsummaries")]
        Task<Response<BittrexSchema.MarketSummariesResponse>> GetMarketSummariesAsync();

        /// <summary>
        /// Used to get the last 24 hour summary of a specific market.
        /// </summary>
        /// <param name="market">The market which summary should be returned.</param>
        /// <returns>Summary for specified market.</returns>
        [Get("/public/getmarketsummary?market={market}")]
        Task<Response<BittrexSchema.MarketSummariesResponse>> GetMarketSummaryAsync([Path] string market);

        /// <summary>
        /// Used to get retrieve the order book for a given market.
        /// </summary>
        /// <param name="market">A string literal for the market (e.g BTC-LTC).</param>
        /// <param name="type">Buy, sell or both to identify the type of order book to return.</param>
        /// <returns>Order book for specified market.</returns>
        [Get("/public/getorderbook")]
        Task<Response<BittrexSchema.OrderBookResponse>> GetOrderBookAsync([Query] string market, [Query] string type = "both");

        /// <summary>
        /// Used to place a buy order in a specific market.
        /// </summary>
        /// <param name="market">A string literal for the market (e.g. BTC-LTC).</param>
        /// <param name="quantity">The amount to purchase.</param>
        /// <param name="rate">The rate at which to place the order.</param>
        /// <returns>Uuid of placed order.</returns>
        [Get("/market/buylimit")]
        Task<Response<BittrexSchema.UuidResponse>> PlaceMarketBuyLimit([Query] string market, [Query] decimal quantity, [Query] decimal rate);

        /// <summary>
        /// Used to place an sell order in a specific market.
        /// </summary>
        /// <param name="market">A string literal for the market (e.g. BTC-LTC).</param>
        /// <param name="quantity">The amount to purchase.</param>
        /// <param name="rate">The rate at which to place the order.</param>
        /// <returns>Uuid of placed order.</returns>
        [Get("/market/selllimit")]
        Task<Response<BittrexSchema.UuidResponse>> PlaceMarketSellLimit([Query] string market, [Query] decimal quantity, [Query] decimal rate);

        /// <summary>
        /// Used to cancel a buy or sell order.
        /// </summary>
        /// <param name="uuid">Uuid of buy or sell order.</param>
        /// <returns>Operation result.</returns>
        [Get("/market/cancel")]
        Task<Response<BittrexSchema.UuidResponse>> CancleOrderAsync([Query] string uuid);

        /// <summary>
        /// Get all orders that you currently have opened. A specific market can be requested.
        /// </summary>
        /// <param name="currencyPair">A string literal for the market (e.g. BTC-LTC)</param>
        /// <returns>Open orders.</returns>
        [Get("/market/getopenorders")]
        Task<Response<BittrexSchema.OpenOrdersResponse>> GetMarketOpenOrders([Query] string currencyPair = null);

        /// <summary>
        /// Used to retrieve a single order by uuid.
        /// </summary>
        /// <param name="uuid">The uuid by which the order should be found.</param>
        /// <returns>Order details.</returns>
        [Get("/account/getorder")]
        Task<Response<BittrexSchema.OrderResponse>> GetAccountOrder([Query] string uuid);

        /// <summary>
        /// Used to retrieve your order history.
        /// </summary>
        /// <param name="currencyPair">A string literal for the market (e.g. BTC-LTC). If omitted, will return for all markets.</param>
        /// <returns>Historical orders.</returns>
        [Get("/account/getorderhistory")]
        Task<Response<BittrexSchema.OrderHistoryResponse>> GetOrderHistory([Query] string currencyPair = null);

        /// <summary>
        /// Used to withdraw funds from your account.
        /// </summary>
        /// <param name="currency">A string literal for the currency (e.g. BTC).</param>
        /// <param name="quantity">The quantity of coins to withdraw.</param>
        /// <param name="address">The address where to send the funds.</param>
        /// <param name="paymentid">Used for CryptoNotes/BitShareX/Nxt optional field (memo/paymentid).</param>
        /// <returns>Withdrawal uuid.</returns>
        [Get("/account/withdraw")]
        Task<Response<BittrexSchema.WithdrawalResponse>> Withdraw([Query] string currency, [Query] decimal quantity, [Query] string address, [Query] string paymentid = null);
    }
}
