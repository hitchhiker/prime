using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Cryptopia;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class CryptopiaTests : ProviderDirectTestsBase
    {
        public CryptopiaTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CryptopiaProvider>().FirstProvider();
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

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // TODO: AY: Cryptopia - test with real money.
            base.PretestPlaceOrderLimit("ETH_BTC".ToAssetPairRaw(), false, 1, new Money(100000, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: AY: Cryptopia - test with real money.
            base.PretestGetTradeOrderStatus("1234", "DOT_BTC".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: AY: Cryptopia - test with real money.
            base.PretestPlaceWithdrawal(new WalletAddress("12498176298371628471628376"), new Money(1, Asset.Btc));
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
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BCH_USDT".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "USD_BTC".ToAssetPairRaw(),
                "BTC_USDT".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "BCH_USDT".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("USD_BTC".ToAssetPairRaw(), true);
        }
    }
}
