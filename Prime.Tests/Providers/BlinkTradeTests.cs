using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BlinkTrade;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BlinkTradeTests : ProviderDirectTestsBase
    {
        public BlinkTradeTests()
        {
            Provider = Networks.I.Providers.OfType<BlinkTradeProvider>().FirstProvider();
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
                "BTC_VND".ToAssetPairRaw(),
                "BTC_BRL".ToAssetPairRaw(),
                "BTC_PKR".ToAssetPairRaw(),
                "BTC_CLP".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_VEF".ToAssetPairRaw(),
                "BTC_VND".ToAssetPairRaw(),
                "BTC_BRL".ToAssetPairRaw(),
                "BTC_PKR".ToAssetPairRaw(),
                "BTC_CLP".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
