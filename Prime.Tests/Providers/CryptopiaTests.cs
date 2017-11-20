using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Cryptopia;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class CryptopiaTests : ProviderDirectTestsBase
    {
        public CryptopiaTests()
        {
            Provider = Networks.I.Providers.OfType<CryptopiaProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BCH_USDT".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BCH_USDT".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
