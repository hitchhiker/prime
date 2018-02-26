using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Plugins.Services.Bittrex
{
    internal interface IBittrexApi
    {
        [Get("/account/getbalances")]
        Task<BittrexSchema.BalancesResponse> GetAllBalancesAsync();

        /// <summary>
        /// Gets or generates new deposit address for specified currency.
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Get("/account/getdepositaddress?currency={currency}")]
        Task<BittrexSchema.DepositAddressResponse> GetDepositAddressAsync([Path] string currency);

        [Get("/public/getmarkets")]
        Task<BittrexSchema.MarketEntriesResponse> GetMarketsAsync();

        [Get("/public/getticker?market={market}")]
        Task<BittrexSchema.TickerResponse> GetTickerAsync([Path] string market);

        [Get("/public/getmarketsummaries")]
        Task<BittrexSchema.MarketSummariesResponse> GetMarketSummariesAsync();

        [Get("/public/getmarketsummary?market={market}")]
        Task<BittrexSchema.MarketSummariesResponse> GetMarketSummaryAsync([Path] string market);

        [Get("/public/getorderbook?market={currencyPair}&type=both")]
        Task<BittrexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currencyPair);

        [Get("/market/buylimit?market={currencyPair}&quantity={quantity}&rate={rate}")]
        Task<BittrexSchema.UuidResponse> GetMarketBuyLimit([Path] string currencyPair, [Path] decimal quantity, [Path] decimal rate);

        [Get("/market/selllimit?market={currencyPair}&quantity={quantity}&rate={rate}")]
        Task<BittrexSchema.UuidResponse> GetMarketSellLimit([Path] string currencyPair, [Path] decimal quantity, [Path] decimal rate);

        [Get("/market/cancel?uuid={uuid}")]
        Task<BittrexSchema.UuidResponse> GetMarketCancel([Path] string uuid);

        [Get("/market/getopenorders")]
        Task<BittrexSchema.OpenOrdersResponse> GetMarketOpenOrders([Query] string currencyPair = null);

        [Get("/account/getorder?uuid={uuid}")]
        Task<BittrexSchema.GetOrderResponse> GetAccountOrder([Path] string uuid);

        [Get("/account/getorderhistory")]
        Task<BittrexSchema.GetOrderHistoryResponse> GetAccountHistory([Query] string currencyPair = null);

        /// <summary>
        /// Used to withdraw funds from your account.
        /// </summary>
        /// <param name="currency">A string literal for the currency (i.e. BTC).</param>
        /// <param name="quantity">The quantity of coins to withdraw.</param>
        /// <param name="address">The address where to send the funds.</param>
        /// <param name="paymentid">Used for CryptoNotes/BitShareX/Nxt optional field (memo/paymentid).</param>
        /// <returns>Returns the withdrawal uuid.</returns>
        [Get("/account/withdraw")]
        Task<BittrexSchema.WithdrawalResponse> Withdraw([Query] string currency, [Query] decimal quantity, [Query] string address, [Query] string paymentid = null);
    }
}
