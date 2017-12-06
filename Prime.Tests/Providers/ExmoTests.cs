using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Exmo;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class ExmoTests : ProviderDirectTestsBase
    {
        public ExmoTests()
        {
            Provider = Networks.I.Providers.OfType<ExmoProvider>().FirstProvider();
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
                "BTC_USD".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "XRP_USD".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_RUB".ToAssetPairRaw(),
                "XRP_USD".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
