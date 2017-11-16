using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.HitBtc;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class HitBtcTests : ProviderDirectTestsBase
    {
        public HitBtcTests()
        {
            Provider = Networks.I.Providers.OfType<HitBtcProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "DOGE_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "DASH_ETH".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            var context = new WalletAddressAssetContext("BTC".ToAssetRaw(), UserContext.Current);
            await base.TestGetAddressesForAssetAsync(context).ConfigureAwait(false);
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
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "DOGE".ToAssetRaw(),
                "NXT".ToAssetRaw(),
                "STEEM".ToAssetRaw()
            }, "BTC".ToAssetRaw());

            await base.TestGetAssetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "DOGE_BTC".ToAssetPairRaw(),
                "NXT_BTC".ToAssetPairRaw(),
                "STEEM_BTC".ToAssetPairRaw()
            });
            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }
    }
}
