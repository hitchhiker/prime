using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coinfloor;
using Prime.Tests.Providers;

namespace Prime.Tests
{
    [TestClass()]
    public class CoinfloorTests : ProviderDirectTestsBase
    {
        public CoinfloorTests()
        {
            Provider = Networks.I.Providers.OfType<CoinfloorProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_PLN".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
                {
                    "BTC_USD".ToAssetPairRaw(),
                    "BTC_GBP".ToAssetPairRaw(),
                    "BTC_EUR".ToAssetPairRaw(),
                    "BTC_PLN".ToAssetPairRaw()
                };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
