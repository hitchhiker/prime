using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.IndependentReserve;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class IndependentReserveTests : ProviderDirectTestsBase
    {
        public IndependentReserveTests()
        {
            Provider = Networks.I.Providers.OfType<IndependentReserveProvider>().FirstProvider();
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
                "btc_usd".ToAssetPairRaw(),
                "btc_aud".ToAssetPairRaw(),
                "btc_nzd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "eth_aud".ToAssetPairRaw(),
                "eth_nzd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw(),
                "bch_aud".ToAssetPairRaw(),
                "bch_nzd".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false, true);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "btc_usd".ToAssetPairRaw(),
                "btc_aud".ToAssetPairRaw(),
                "btc_nzd".ToAssetPairRaw(),
                "eth_usd".ToAssetPairRaw(),
                "eth_aud".ToAssetPairRaw(),
                "eth_nzd".ToAssetPairRaw(),
                "bch_usd".ToAssetPairRaw(),
                "bch_aud".ToAssetPairRaw(),
                "bch_nzd".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook("btc_usd".ToAssetPairRaw(), false);
        }
    }
}
