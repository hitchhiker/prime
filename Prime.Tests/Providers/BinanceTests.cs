using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Core;
using Prime.Plugins.Services.Binance;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BinanceTests : ProviderDirectTestsBase
    {
        public BinanceTests()
        {
            Provider = Networks.I.Providers.OfType<BinanceProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("LTC_BTC".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, true).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
            {
                "NEO_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "NEO_ETH".ToAssetPairRaw(),
                "IOTA_BTC".ToAssetPairRaw(),
                "ETC_BTC".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "BNT".ToAssetRaw(),
                "ETC".ToAssetRaw()
            }, "BTC".ToAssetRaw());

            await base.TestGetAssetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BNT_ETH".ToAssetPairRaw(),
                "BNT_BTC".ToAssetPairRaw(),
                "SALT_ETH".ToAssetPairRaw(),
                "SALT_BTC".ToAssetPairRaw()
            });

            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            var context = new OrderBookContext(new AssetPair("BNT".ToAssetRaw(), "BTC".ToAssetRaw()));
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);

            context = new OrderBookContext(new AssetPair("BNT".ToAssetRaw(), "BTC".ToAssetRaw()), 20);
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            var context = new OhlcContext(new AssetPair("BNT", "BTC"), TimeResolution.Minute,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute));
            await base.TestGetOhlcAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            var context = new VolumeContext()
            {
                Pair = "BNT_BTC".ToAssetPairRaw()
            };

            await base.TestGetVolumeAsync(context).ConfigureAwait(false);
        }
    }
}
