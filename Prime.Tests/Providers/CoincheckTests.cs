using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Coincheck;

namespace Prime.Tests.Providers
{

    [TestClass()]
    public class CoincheckTests : ProviderDirectTestsBase
    {
        public CoincheckTests()
        {
            Provider = Networks.I.Providers.OfType<CoincheckProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
        {
            "btc_jpy".ToAssetPairRaw(),
            "eth_jpy".ToAssetPairRaw(),
            "etc_jpy".ToAssetPairRaw(),
            "dao_jpy".ToAssetPairRaw(),
            "lsk_jpy".ToAssetPairRaw(),
            "fct_jpy".ToAssetPairRaw(),
            "xmr_jpy".ToAssetPairRaw(),
            "rep_jpy".ToAssetPairRaw(),
            "xrp_jpy".ToAssetPairRaw(),
            "zec_jpy".ToAssetPairRaw(),
            "xem_jpy".ToAssetPairRaw()
        };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
        {
            "btc_jpy".ToAssetPairRaw(),
            "eth_jpy".ToAssetPairRaw(),
            "etc_jpy".ToAssetPairRaw(),
            "dao_jpy".ToAssetPairRaw(),
            "lsk_jpy".ToAssetPairRaw(),
            "fct_jpy".ToAssetPairRaw(),
            "xmr_jpy".ToAssetPairRaw(),
            "rep_jpy".ToAssetPairRaw(),
            "xrp_jpy".ToAssetPairRaw(),
            "zec_jpy".ToAssetPairRaw(),
            "xem_jpy".ToAssetPairRaw()
        };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
