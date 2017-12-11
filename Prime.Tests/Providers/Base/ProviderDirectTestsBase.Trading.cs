using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
using Prime.Common;

namespace Prime.Tests.Providers
{
    public abstract partial class ProviderDirectTestsBase
    {
        #region Wrappers

        public virtual void TestGetTradeOrderStatus() { }
        public void TestGetTradeOrderStatus(string remoteOrderId)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                GetTradeOrderStatus(p.Provider, remoteOrderId);
        }

        public virtual void TestPlaceOrderLimit() { }
        public void TestPlaceOrderLimit(AssetPair market, bool isBuy, decimal quantity, Money rate)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                PlaceOrderLimit(p.Provider, market, isBuy, quantity, rate);
        }

        public virtual void TestGetBalances()
        {
            var p = IsType<IBalanceProvider>();
            if (p.Success)
                GetBalances(p.Provider);
        }

        #endregion

        #region Tests

        private void GetTradeOrderStatus(IOrderLimitProvider provider, string remoteOrderId)
        {
            try
            {
                var context = new RemoteIdContext(UserContext.Current, remoteOrderId);
                var r = AsyncContext.Run(() => provider.GetOrderStatusAsync(context));

                Assert.IsTrue(remoteOrderId.Equals(r.RemoteOrderId, StringComparison.Ordinal), "Remote trade order ids don't match");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void PlaceOrderLimit(IOrderLimitProvider provider, AssetPair market, bool isBuy, decimal quantity, Money rate)
        {
            try
            {
                var context = new PlaceOrderLimitContext(UserContext.Current, market, isBuy, quantity, rate);

                var r = AsyncContext.Run(() => provider.PlaceOrderLimitAsync(context));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        private void GetBalances(IBalanceProvider provider)
        {
            var ctx = new NetworkProviderPrivateContext(UserContext.Current);

            try
            {
                var balances = AsyncContext.Run(() => provider.GetBalancesAsync(ctx));

                Assert.IsTrue(balances != null);

                Trace.WriteLine("User balances: ");
                foreach (var b in balances)
                {
                    Trace.WriteLine($"{b.Asset}: {b.Available} available, {b.AvailableAndReserved} balance, {b.Reserved} reserved");
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #endregion
    }
}
