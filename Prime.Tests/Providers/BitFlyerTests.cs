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
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "BTC_JPY".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            await base.TestGetPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()), 20);
            await base.TestGetOrderBookAsync();
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            var ctx = new VolumeContext()
            {
                Pair = "BTC_JPY".ToAssetPairRaw()
            };
            GetVolumeFunc = () => ((BitFlyerProvider)Provider).GetVolumeAsync(ctx);

            await base.TestGetVolumeAsync();
        }
    }
}
