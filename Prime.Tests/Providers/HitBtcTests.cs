using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.HitBtc;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class HitBtcTests : ProviderDirectTestsBase
    {
        public HitBtcTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<HitBtcProvider>().FirstProvider();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DOGE_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "DASH_ETH".ToAssetPairRaw(),
            };

            base.PretestGetPricing(pairs, false, false);
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DOGE_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "DASH_ETH".ToAssetPairRaw(),
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact] 
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext("BTC".ToAssetRaw(), UserContext.Current);
            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: AY: test using real money - HitBtc.
            base.PretestGetTradeOrderStatus("orderid");
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // TODO: AY: test using real money - HitBtc.
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), true, 1, new Money(1000m, Asset.Usd));
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: test using real money - HitBtc.
            base.PretestPlaceWithdrawal(new WalletAddress("123"), new Money(1, Asset.Btc));
        }

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }
    }
}
