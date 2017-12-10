using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coinmate;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CoinmateTests : ProviderDirectTestsBase
    {
        public CoinmateTests()
        {
            Provider = Networks.I.Providers.OfType<CoinmateProvider>().FirstProvider();
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
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_CZK".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_CZK".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
