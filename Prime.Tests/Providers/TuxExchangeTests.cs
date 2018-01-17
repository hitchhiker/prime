using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.TuxExchange;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class TuxExchangeTests : ProviderDirectTestsBase
    {
        public TuxExchangeTests()
        {
            Provider = Networks.I.Providers.OfType<TuxExchangeProvider>().FirstProvider();
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
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_ICN".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_ICN".ToAssetPairRaw(),
                "BTC_PPC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }


        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("DOGE_BTC".ToAssetPairRaw(), true);
        }
    }
}
