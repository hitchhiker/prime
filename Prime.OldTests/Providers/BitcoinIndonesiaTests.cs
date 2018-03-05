using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitcoinIndonesia;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BitcoinIndonesiaTests : ProviderDirectTestsBase
    {
        public BitcoinIndonesiaTests()
        {
            Provider = Networks.I.Providers.OfType<BitcoinIndonesiaProvider>().FirstProvider();
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
                "BTC_IDR".ToAssetPairRaw(),
                "NXT_IDR".ToAssetPairRaw(),
                "BCH_IDR".ToAssetPairRaw(),
                "NXT_BTC".ToAssetPairRaw(),
                "BTG_IDR".ToAssetPairRaw(),
                "XRP_IDR".ToAssetPairRaw(),
                "ETH_IDR".ToAssetPairRaw(),
                "ETC_IDR".ToAssetPairRaw(),
                "WAVES_IDR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
                "BTS_BTC".ToAssetPairRaw(),
                "XZC_IDR".ToAssetPairRaw(),
                "LTC_IDR".ToAssetPairRaw(),
                "DOGE_BTC".ToAssetPairRaw(),
                "DRK_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "NEM_BTC".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false, false);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_IDR".ToAssetPairRaw(),
                "NXT_IDR".ToAssetPairRaw(),
                "BCH_IDR".ToAssetPairRaw(),
                "NXT_BTC".ToAssetPairRaw(),
                "BTG_IDR".ToAssetPairRaw(),
                "XRP_IDR".ToAssetPairRaw(),
                "ETH_IDR".ToAssetPairRaw(),
                "ETC_IDR".ToAssetPairRaw(),
                "WAVES_IDR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
                "BTS_BTC".ToAssetPairRaw(),
                "XZC_IDR".ToAssetPairRaw(),
                "LTC_IDR".ToAssetPairRaw(),
                "DOGE_BTC".ToAssetPairRaw(),
                "DRK_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "NEM_BTC".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
