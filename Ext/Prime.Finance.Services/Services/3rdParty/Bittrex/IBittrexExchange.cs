using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bittrex
{
    public interface IExchange
    {
        void Initialise(ExchangeContext context);
        AccountBalance GetBalance(string market);
        GetBalancesResponse GetBalances();
        decimal CalculateMinimumOrderQuantity(string market, decimal price);
        dynamic GetMarkets();
        dynamic GetTicker(string market);
        GetOpenOrdersResponse GetOpenOrders(string market);        
        OrderResponse PlaceBuyOrder(string market, decimal quantity, decimal price);
        OrderResponse PlaceSellOrder(string market, decimal quantity, decimal price);
        void CancelOrder(string uuid);

        /// <summary>
        /// Used to retrieve the orderbook for a given market
        /// </summary>
        /// <param name="market"></param>
        /// <param name="type">The type of orderbook to return.</param>
        /// <param name="depth">How deep of an order book to retrieve. Max is 50</param>
        /// <returns></returns>
        GetOrderBookResponse GetOrderBook(string market, OrderBookType type, int depth = 20);

        /// <summary>
        /// Used to retrieve the latest trades that have occured for a specific market.
        /// </summary>
        /// <param name="market"></param>
        /// <param name="count">a number between 1-50 for the number of entries to return</param>
        /// <returns></returns>
        GetMarketHistoryResponse GetMarketHistory(string market, int count = 20);

        GetMarketSummaryResponse GetMarketSummary(string market);

        GetOrderHistoryResponse GetOrderHistory(string market, int count = 10);
    }
}
