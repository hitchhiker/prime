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
        Task<BittrexSchema.BalancesResponse> GetAllBalances();

        /// <summary>
        /// Gets or generates new deposit address for specified currency.
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Get("/account/getdepositaddress?currency={currency}")]
        Task<BittrexSchema.DepositAddressResponse> GetDepositAddress([Path] string currency);

        [Get("/public/getmarkets")]
        Task<BittrexSchema.MarketEntriesResponse> GetMarkets();

        [Get("/public/getticker?market={market}")]
        Task<BittrexSchema.TickerResponse> GetTicker([Path] string market);

        [Get("/public/getmarketsummaries")]
        Task<BittrexSchema.MarketSummariesResponse> GetMarketSummaries();

        [Get("/public/getmarketsummary?market={market}")]
        Task<BittrexSchema.MarketSummaryResponse> GetMarketSummary([Path] string market);

        [Get("/public/getorderbook?market={currenctPair}&type=both")]
        Task<BittrexSchema.OrderBookResponse> GetOrderBook([Path] string currenctPair);
    }
}
