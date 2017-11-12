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

        [Get("/public/getorderbook?market={currenctPair}&type=both")]
        Task<BittrexSchema.OrderBookResponse> GetOrderBookAsync([Path] string currenctPair);
    }
}
