using System;
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
        
        public virtual void TestGetOrdersHistory() { }
        public void PretestGetOrdersHistory(AssetPair market = null)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                GetOrdersHistoryTest(p.Provider, market);
        }
        
        public virtual void TestGetOpenOrders() { }
        public void PretestGetOpenOrders(AssetPair market = null)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                GetOpenOrdersTest(p.Provider, market);
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

        private void DisplayOrderStatusInfo(TradeOrderStatus tradeOrderStatus, AssetPair market)
        {
            OutputWriter.WriteLine($"Remote trade order id: {tradeOrderStatus.RemoteOrderId}");

            var marketString = tradeOrderStatus.HasMarket ? tradeOrderStatus.Market: market;
            OutputWriter.WriteLine($"{(tradeOrderStatus.IsBuy ? "Buy" : "Sell")} {tradeOrderStatus.AmountInitial ?? Decimal.MinValue} for {tradeOrderStatus.Rate ?? Decimal.MinValue} on '{marketString}' market");

            if (tradeOrderStatus.AmountFilled.HasValue) OutputWriter.WriteLine($"Filled amount is {tradeOrderStatus.AmountFilled.Value}");
            if (tradeOrderStatus.AmountRemaining.HasValue) OutputWriter.WriteLine($"Remaining amount is {tradeOrderStatus.AmountRemaining.Value}");

            if (tradeOrderStatus.IsOpen) OutputWriter.WriteLine("Order is open");

            if (tradeOrderStatus.IsCanceled) OutputWriter.WriteLine("Order is canceled");
            if (tradeOrderStatus.IsClosed) OutputWriter.WriteLine("Order is closed");

            if (tradeOrderStatus.IsCancelRequested) OutputWriter.WriteLine("Order is requested to be canceled");

            OutputWriter.WriteLine("");
        }

        private void DisplayOrderStatusInfo(TradeOrderStatus tradeOrderStatus)
        {
            DisplayOrderStatusInfo(tradeOrderStatus, AssetPair.Empty);
        }

        private void GetTradeOrderStatusTest(IOrderLimitProvider provider, string remoteOrderId, AssetPair market = null)
        {
            var context = new RemoteMarketIdContext(UserContext.Current, remoteOrderId, market);
            
            var r = AsyncContext.Run(() => provider.GetOrderStatusAsync(context)).TradeOrderStatus;

            Assert.True(remoteOrderId.Equals(r.RemoteOrderId, StringComparison.Ordinal), "Remote trade order ids don't match");

            DisplayOrderStatusInfo(r, market);
        }

        private void GetOpenOrdersTest(IOrderLimitProvider provider, AssetPair market = null)
        {
            var context = new OpenOrdersContext(UserContext.Current)
            {
                Market = market
            };

            OutputWriter.WriteLine("Open orders:\n");

            var orders = AsyncContext.Run(() => provider.GetOpenOrdersAsync(context)).Orders.ToArray();

            if (orders.Length == 0)
                OutputWriter.WriteLine("No open orders returned");
            else
                orders.ForEach(DisplayOrderStatusInfo);
        }

        private void GetOrdersHistoryTest(IOrderLimitProvider provider, AssetPair market = null)
        {
            var context = new TradeOrdersContext(UserContext.Current)
            {
                Market = market
            };

            OutputWriter.WriteLine("Orders history: \n");

            var orders = AsyncContext.Run(() => provider.GetOrdersHistory(context)).Orders.ToArray();

            if (orders.Length == 0)
                OutputWriter.WriteLine("No trade orders returned");
            else
                orders.ForEach(DisplayOrderStatusInfo);
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
