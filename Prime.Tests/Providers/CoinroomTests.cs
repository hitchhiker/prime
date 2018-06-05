using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Coinroom;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class CoinroomTests : ProviderDirectTestsBase
    {
        public CoinroomTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CoinroomProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
        {
            "BTC_USD".ToAssetPairRaw(),
            "LTC_EUR".ToAssetPairRaw(),
            "BTC_EUR".ToAssetPairRaw()
        };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
        {
            "BTC_USD".ToAssetPairRaw(),
            "LTC_EUR".ToAssetPairRaw(),
            "BTC_EUR".ToAssetPairRaw()
        };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            // TODO: SC: Not tested with real money
            base.PretestPlaceWithdrawal(new WalletAddress("123"), new Money(1, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "BTC_USD".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            //TODO: SC: Not tested with real money
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), false, new Money(10, Asset.Usd), new Money(1m, Asset.Usd));
        }
    }
}
