using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
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

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            await base.TestGetAddressesAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "ETH_ETC".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "USDT_BTC".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            await base.TestGetAddressesForAssetAsync();
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            PublicPriceContext = new PublicPriceContext(new AssetPair("BTC", "LTC"));
            await base.TestGetPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            PublicAssetPricesContext = new PublicAssetPricesContext(new List<Asset>()
            {
                "BTC".ToAssetRaw(),
                "USDT".ToAssetRaw(),
                "XMR".ToAssetRaw()
            }, "LTC".ToAssetRaw());

            await base.TestGetAssetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            PublicPricesContext = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_NXT".ToAssetPairRaw(),
                "BTC_ETH".ToAssetPairRaw()
            });

            await base.TestGetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            // BUG: supports only 1 day.
            OhlcContext = new OhlcContext(new AssetPair("BTC", "LTC"), TimeResolution.Day,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Hour), null);

            await base.TestGetOhlcAsync();
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            var ctx = new VolumeContext()
            {
                Pair = "BTC_USD".ToAssetPairRaw()
            };
            GetVolumeFunc = () => ((PoloniexProvider)Provider).GetVolumeAsync(ctx);

            await base.TestGetVolumeAsync();
        }
    }
}
