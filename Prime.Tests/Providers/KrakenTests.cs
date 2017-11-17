using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Kraken;
using AssetPair = Prime.Common.AssetPair;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class KrakenTests : ProviderDirectTestsBase
    {
        public KrakenTests()
        {
            Provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
        }

        [TestMethod]
        public async Task TestAllAssetPairsPrices()
        {
            var pairs = await ((IAssetPairsProvider) Provider).GetAssetPairsAsync(new NetworkProviderContext()).ConfigureAwait(false);

            var prices = await ((IPublicPricesProvider) Provider).GetPricesAsync(new PublicPricesContext(pairs.ToList()))
                .ConfigureAwait(false);

            Assert.IsTrue(!prices.MissedPairs.Any());
            Assert.IsTrue(pairs.Count == prices.MarketPrices.Count);
        }

        [TestMethod]
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("LTC_BTC".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, true).ConfigureAwait(false);

            context = new PublicPriceContext("BCH_EUR".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "LTC".ToAssetRaw(),
                "XRP".ToAssetRaw()
            }, "BTC".ToAssetRaw());

            await base.TestGetAssetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            });

            await base.TestGetPricesAsync(context).ConfigureAwait(false);

            context = new PublicPricesContext(new List<AssetPair>()
            {
                "BCH_EUR".ToAssetPairRaw(),
                "BCH_USD".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "EOS_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw(),
                "GNO_ETH".ToAssetPairRaw(),
                "GNO_BTC".ToAssetPairRaw(),
                "DASH_EUR".ToAssetPairRaw(),
                "DASH_USD".ToAssetPairRaw(),
                "DASH_BTC".ToAssetPairRaw()
            });

            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.

            var context = new WalletAddressContext(UserContext.Current);
            await base.TestGetAddressesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.
            var context = new WalletAddressAssetContext("MLN".ToAssetRaw(), UserContext.Current);
            await base.TestGetAddressesForAssetAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            var context = new OhlcContext("ETC_ETH".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            await base.TestGetOhlcAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var pairs = new AssetPairs()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync(pairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()));
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);
        }
    }
}
