using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.BitcoinIndonesia;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitcoinIndonesiaTests : ProviderDirectTestsBase
    {
        public BitcoinIndonesiaTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitcoinIndonesiaProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
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

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
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

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
