using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Kuna;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class KunaTests : ProviderDirectTestsBase
    {
        public KunaTests()
        {
            Provider = Networks.I.Providers.OfType<KunaProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "btc_uah".ToAssetPairRaw(),
                "eth_uah".ToAssetPairRaw(),
                "kun_btc".ToAssetPairRaw(),
                "wave_suah".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_uah".ToAssetPairRaw(),
                "eth_uah".ToAssetPairRaw(),
                "kun_btc".ToAssetPairRaw(),
                "wave_suah".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
