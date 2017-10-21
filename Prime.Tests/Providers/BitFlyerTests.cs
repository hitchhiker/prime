using System;
using System.Collections.Generic;
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
            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetLatestPriceAsync()
        {
            await base.TestGetLatestPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetLatestPricesAsync()
        {
            PublicPricesContext = new PublicPricesContext(Asset.Btc, new List<Asset>()
            {
                "USD".ToAssetRaw(),
                "JPY".ToAssetRaw()
            });

            await base.TestGetLatestPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "JPY".ToAssetRaw()), 20);
            await base.TestGetOrderBookAsync();
        }
    }
}
