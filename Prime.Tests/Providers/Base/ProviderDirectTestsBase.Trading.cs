using System;
using System.Diagnostics;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Xunit;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {
        #region Wrappers

        public virtual void TestGetTradeOrderStatus() { }
        public void PretestGetTradeOrderStatus(string remoteOrderId, AssetPair market = null)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                GetTradeOrderStatusTest(p.Provider, remoteOrderId, market);
        }
        
        public virtual void TestGetTradeOrders() { }

        public void PretestGetTradeOrders()
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                GetTradeOrdersTest(p.Provider);
        }

        public virtual void TestPlaceOrderLimit() { }
        public void PretestPlaceOrderLimit(AssetPair market, bool isBuy, Money quantity, Money rate)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                PlaceOrderLimitTest(p.Provider, market, isBuy, quantity, rate);
        }

        public virtual void TestGetMarketFromOrder() { }
        public void PretestGetMarketFromOrder(string remoteOrderId)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                GetMarketFromOrderTest(p.Provider, remoteOrderId);
        }

        public virtual void TestGetBalances()
        {
            var p = IsType<IBalanceProvider>();
            if (p.Success)
                GetBalancesTest(p.Provider);
        }

        #endregion

        #region Tests

        private void DisplayOrderStatusInfo(TradeOrderStatus tradeOrderStatus)
        {
            OutputWriter.WriteLine($"Remote trade order id: {tradeOrderStatus.RemoteOrderId}");
            OutputWriter.WriteLine($"Order side: {(tradeOrderStatus.IsBuy ? "buy" : "sell")}");

            if (tradeOrderStatus.IsOpen) OutputWriter.WriteLine("Order is open");
            if (tradeOrderStatus.IsCancelRequested) OutputWriter.WriteLine("Order is requested to be canceled");
            if (tradeOrderStatus.IsCanceled) OutputWriter.WriteLine("Order is canceled");
            if (tradeOrderStatus.IsClosed) OutputWriter.WriteLine("Order is closed");
            if (tradeOrderStatus.IsFound) OutputWriter.WriteLine("Order is found");

            if(tradeOrderStatus.HasMarket) OutputWriter.WriteLine($"The market is '{tradeOrderStatus.Market}'");
            if (tradeOrderStatus.Rate.HasValue) OutputWriter.WriteLine($"The rate of order is {tradeOrderStatus.Rate.Value}");
            if (tradeOrderStatus.AmountInitial.HasValue) OutputWriter.WriteLine($"Initial amount is {tradeOrderStatus.AmountInitial.Value}");
            if (tradeOrderStatus.AmountFilled.HasValue) OutputWriter.WriteLine($"Filled amount is {tradeOrderStatus.AmountFilled.Value}");
            if (tradeOrderStatus.AmountRemaining.HasValue) OutputWriter.WriteLine($"Remaining amount is {tradeOrderStatus.AmountRemaining.Value}");

            OutputWriter.WriteLine("");
        }

        private void GetTradeOrderStatusTest(IOrderLimitProvider provider, string remoteOrderId, AssetPair market = null)
        {
            var context = new RemoteMarketIdContext(UserContext.Current, remoteOrderId, market);
            
            var r = AsyncContext.Run(() => provider.GetOrderStatusAsync(context)).TradeOrderStatus;

            Assert.True(remoteOrderId.Equals(r.RemoteOrderId, StringComparison.Ordinal), "Remote trade order ids don't match");

            DisplayOrderStatusInfo(r);
        }

        private void GetTradeOrdersTest(IOrderLimitProvider provider)
        {
            var context = new TradeOrdersContext(UserContext.Current);

            var r = AsyncContext.Run(() => provider.GetTradeOrdersAsync(context));

            foreach (var tradeOrderStatus in r.Orders)
            {
                DisplayOrderStatusInfo(tradeOrderStatus);
            }
        }

        private void PlaceOrderLimitTest(IOrderLimitProvider provider, AssetPair market, bool isBuy, Money quantity, Money rate)
        {
            var context = new PlaceOrderLimitContext(UserContext.Current, market, isBuy, quantity, rate);

            var r = AsyncContext.Run(() => provider.PlaceOrderLimitAsync(context));

            Assert.True(!String.IsNullOrWhiteSpace(r.RemoteOrderGroupId), "Remote id is empty.");
            OutputWriter.WriteLine($"Remote trade order id: {r.RemoteOrderGroupId}");
        }

        private void GetMarketFromOrderTest(IOrderLimitProvider provider, string remoteOrderId)
        {
            var context = new RemoteIdContext(UserContext.Current, remoteOrderId);

            var r = AsyncContext.Run(() => provider.GetMarketFromOrderAsync(context));

            OutputWriter.WriteLine($"Remote trade order id is {remoteOrderId}, market is {r.Market}");

            Assert.True(r.Market != null, "Returned market is null");
            Assert.True(!Equals(r.Market, AssetPair.Empty), "Returned market is AssetPair.Empty");
        }

        private void GetBalancesTest(IBalanceProvider provider)
        {
            var ctx = new NetworkProviderPrivateContext(UserContext.Current);

            var balances = AsyncContext.Run(() => provider.GetBalancesAsync(ctx));

            Assert.True(balances != null);

            OutputWriter.WriteLine("User balances: ");
            foreach (var b in balances.OrderByDescending(x => x.AvailableAndReserved.ToDecimalValue()))
            {
                OutputWriter.WriteLine($"{b.Asset}: {b.Available} available, {b.Reserved} reserved, {b.AvailableAndReserved} total");
            }
        }

        #endregion
    }
}
