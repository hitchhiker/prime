using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.NLexch;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class NLexchTests : ProviderDirectTestsBase
    {
        public NLexchTests()
        {
            Provider = Networks.I.Providers.OfType<NLexchProvider>().FirstProvider();
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
                "doge_btc".ToAssetPairRaw(),
                "uis_btc".ToAssetPairRaw(),
                "ltc_btc".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "doge_btc".ToAssetPairRaw(),
                "uis_btc".ToAssetPairRaw(),
                "ltc_btc".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
