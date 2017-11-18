using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitFlyer;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BitFlyerTests : ProviderDirectTestsBase
    {
        public BitFlyerTests()
        {
            Provider = Networks.I.Providers.OfType<BitFlyerProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestGetPricing()
        { 
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var context = new AssetPairs()
            {
                "BTC_JPY".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("ETH_BTC".ToAssetPairRaw());
            await base.TestGetPrice(context, true).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()));
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()), 20);
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);
        }
    }
}
