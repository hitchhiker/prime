using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitBay;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BitBayTests : ProviderDirectTestsBase
    {
        public BitBayTests()
        {
            Provider = Networks.I.Providers.OfType<BitBayProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "BTC_PLN".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "ETH_PLN".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "LSK_PLN".ToAssetPairRaw(),
                "LSK_USD".ToAssetPairRaw(),
                "LSK_EUR".ToAssetPairRaw(),
                "BCC_PLN".ToAssetPairRaw(),
                "BCC_USD".ToAssetPairRaw(),
                "BCC_EUR".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "BTC_PLN".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "ETH_PLN".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "LSK_PLN".ToAssetPairRaw(),
                "LSK_USD".ToAssetPairRaw(),
                "LSK_EUR".ToAssetPairRaw(),
                "BCC_PLN".ToAssetPairRaw(),
                "BCC_USD".ToAssetPairRaw(),
                "BCC_EUR".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }
    }
}
