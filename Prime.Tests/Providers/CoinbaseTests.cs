using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Coinbase;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class CoinbaseTests : ProviderDirectTestsBase
    {
        public CoinbaseTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<CoinbaseProvider>().FirstProvider();
        }

        #region Public

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
                "BTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "BTC_GBP".ToAssetPairRaw(),
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("LTC_EUR".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            base.PretestGetOhlc(context);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }

        #endregion

        #region Private

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
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Current);

            base.PretestGetAddresses(context);
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // AY: Not tested.
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), true, new Money(1, Asset.Btc), new Money(1, Asset.Usd));

            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), false, new Money(1, Asset.Btc), new Money(100000, Asset.Usd));
        }

        [Fact]
        public override void TestGetOpenOrders()
        {
            base.PretestGetOpenOrders();
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            base.PretestGetOrdersHistory();
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("test");
        }

        #endregion
    }
}
