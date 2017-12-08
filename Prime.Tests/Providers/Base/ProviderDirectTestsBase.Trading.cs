using System;
using System.Collections.Generic;
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
        public virtual void TestGetTradeOrderStatus() { }

        public virtual void TestGetTradeOrderStatus(string remoteOrderId)
        {
            var p = IsType<IOrderLimitProvider>();
            if (p.Success)
                GetTradeOrderStatus(p.Provider, remoteOrderId);
        }

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
    }
}
