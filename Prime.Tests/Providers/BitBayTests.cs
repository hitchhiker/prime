using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.BitBay;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitBayTests : ProviderDirectTestsBase
    {
        public BitBayTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitBayProvider>().FirstProvider();
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
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "BTC_PLN".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "ETH_PLN".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "LSK_PLN".ToAssetPairRaw(),
                "LSK_USD".ToAssetPairRaw(),
                "LSK_EUR".ToAssetPairRaw(),
                "BCC_PLN".ToAssetPairRaw(),
                "BCC_USD".ToAssetPairRaw(),
                "BCC_EUR".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "LTC_PLN".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "BTC_PLN".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "ETH_PLN".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "LSK_PLN".ToAssetPairRaw(),
                "LSK_USD".ToAssetPairRaw(),
                "LSK_EUR".ToAssetPairRaw(),
                "BCC_PLN".ToAssetPairRaw(),
                "BCC_USD".ToAssetPairRaw(),
                "BCC_EUR".ToAssetPairRaw()
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
            // TODO: SC: Not tested with real money.
            base.PretestPlaceWithdrawal(new WalletAddress("1234"), new Money(22, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            // TODO: SC: Not tested with real money.
            var orderId = "21109502";
            base.PretestGetTradeOrderStatus(orderId, "BTC_USD".ToAssetPairRaw());
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // TODO: SC: Not tested with real money.
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), false, new Money(1, Asset.Usd), new Money(1m, Asset.Usd));
        }
    }
}
