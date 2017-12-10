using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Bitfinex;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BitfinexTests : ProviderDirectTestsBase
    {
        public BitfinexTests()
        {
            Provider = Networks.I.Providers.OfType<BitfinexProvider>().FirstProvider();
        }

        #region Private

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        #endregion

        #region Public

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
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        #endregion
    }
}
