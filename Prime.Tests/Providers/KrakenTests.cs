using System.Collections.Generic;
using System.Linq;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Plugins.Services.Kraken;
using Xunit;
using AssetPair = Prime.Common.AssetPair;

namespace Prime.Tests.Providers
{
    public class KrakenTests : ProviderDirectTestsBase
    {
        public KrakenTests()
        {
            Provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
        }

        [Fact]
        public void TestAllAssetPairsPrices()
        {
            var pairs = AsyncContext.Run(() => (IAssetPairsProvider) Provider).GetAssetPairsAsync(new NetworkProviderContext()).Result;

            var prices = AsyncContext.Run(() => (IPublicPricingProvider) Provider).GetPricingAsync(new PublicPricesContext(pairs.ToList())).Result;

            Assert.True(!prices.MissedPairs.Any());
            Assert.True(pairs.Count == prices.Count);
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "ETH_BTC".ToAssetPairRaw(),
                "ETC_ETH".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [Fact]
        public override void TestGetAddresses()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.

            var context = new WalletAddressContext(UserContext.Current);
            base.PretestGetAddresses(context);
        }

        [Fact]
        public override void TestGetAddressesForAsset()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.
            var context = new WalletAddressAssetContext("MLN".ToAssetRaw(), UserContext.Current);
            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("ETC_ETH".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            base.PretestGetOhlc(context);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var pairs = new AssetPairs()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            };

            base.PretestGetAssetPairs(pairs);
        }

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false, 50);
        }
    }
}
