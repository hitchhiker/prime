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
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DOGE_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "DASH_ETH".ToAssetPairRaw(),
            };

            base.TestGetPricing(pairs, false, false);
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DOGE_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "DASH_ETH".ToAssetPairRaw(),
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext("BTC".ToAssetRaw(), UserContext.Current);
            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestApi()
        {
            base.TestApi();
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }
    }
}
