using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Bitso;
using Xunit;

namespace Prime.Tests.Providers
{
    public class BitsoTests : ProviderDirectTestsBase
    {
        public BitsoTests()
        {
            Provider = Networks.I.Providers.OfType<BitsoProvider>().FirstProvider();
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
                "eth_btc".ToAssetPairRaw(),
                "eth_mxn".ToAssetPairRaw(),
                "xrp_btc".ToAssetPairRaw(),
                "xrp_mxn".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };
            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "eth_mxn".ToAssetPairRaw(),
                "xrp_btc".ToAssetPairRaw(),
                "xrp_mxn".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw(),
                "bch_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("eth_btc".ToAssetPairRaw(), true);
        }
    }
}
