using Rokolab.BitstampClient.Models;
using System.Collections.Generic;
using Prime.Core;

namespace Rokolab.BitstampClient
{
    public interface IBitstampClient
    {
        TickerResponse GetTicker();
        Dictionary<string,string> GetBalance();
        OrderStatusResponse GetOrderStatus(string orderId);
        List<OpenOrderResponse> GetOpenOrders();
        WalletAddress GetDepositAddress(IWalletService service, Asset asset);
        List<TransactionResponse> GetTransactions(int offset, int limit);
        bool CancelOrder(string orderId);
        bool CancelAllOrders();
        BuySellResponse Buy(double amount, double price);
        BuySellResponse Sell(double amount, double price);
    }
}