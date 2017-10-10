using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Plugins.Services.Kraken;
using Prime.Plugins.Services.Poloniex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class PoloniexTests : ProviderDirectTestsBase
    {
        public PoloniexTests()
        {
            Provider = Networks.I.Providers.OfType<PoloniexProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "NXT".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "NXT".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync();
        }
    }
}
