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
            await base.TestApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "NXT".ToAssetRaw()));
            await TestGetOrderBookAsync(context).ConfigureAwait(false);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "NXT".ToAssetRaw()), 100);
            await TestGetOrderBookAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            var context = new WalletAddressContext(UserContext.Current);
            await TestGetAddressesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "ETH_ETC".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
                "USDT_BTC".ToAssetPairRaw(),
            };

            await TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            await base.TestGetAddressesForAssetAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var c = new PublicPriceContext(new AssetPair("USDT", "BTC"));
            await TestGetPriceAsync(c, true).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "BTC".ToAssetRaw(),
                "USDT".ToAssetRaw(),
                "XMR".ToAssetRaw()
            }, "LTC".ToAssetRaw());

            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_NXT".ToAssetPairRaw(),
                "BTC_ETH".ToAssetPairRaw()
            });

            // TODO: re-implement.
            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            // BUG: supports only 1 day.
            var context = new OhlcContext(new AssetPair("BTC", "LTC"), TimeResolution.Day,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Hour), null);

            await base.TestGetOhlcAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            var context = new VolumeContext()
            {
                Pair = "BTC_LTC".ToAssetPairRaw()
            };

            await base.TestGetVolumeAsync(context).ConfigureAwait(false);
        }
    }
}
