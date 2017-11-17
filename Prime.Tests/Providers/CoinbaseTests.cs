using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using plugins;
using Prime.Common;
using Prime.Plugins.Services.Coinbase;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CoinbaseTests : ProviderDirectTestsBase
    {
        public CoinbaseTests()
        {
            Provider = Networks.I.Providers.OfType<CoinbaseProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
            {
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }
        
        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            var context = new OhlcContext("LTC_EUR".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            await base.TestGetOhlcAsync(context).ConfigureAwait(false);
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
        public override async Task TestGetAddressesForAssetAsync()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            await base.TestGetAddressesForAssetAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            var context = new WalletAddressContext(UserContext.Current);

            await base.TestGetAddressesAsync(context).ConfigureAwait(false);
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
