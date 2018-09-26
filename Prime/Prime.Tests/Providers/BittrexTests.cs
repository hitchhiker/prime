using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Bittrex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BittrexTests : ProviderDirectTestsBase
    {
        public BittrexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
        }

        #region Public

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_XRP".ToAssetPairRaw(),
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_ETH".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
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
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_XRP".ToAssetPairRaw(),
                "BTC_ETH".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_XRP".ToAssetPairRaw(), false);
        }

        #endregion

        #region Private

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Testing);

            base.PretestGetAddresses(context);
        }

        [Fact]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext("BTC".ToAssetRaw(), UserContext.Testing);

            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // Reliable test for selling.
            base.PretestPlaceOrderLimit("BTC_XRP".ToAssetPairRaw(), false, new Money(3m, Asset.Xrp), new Money(1m, Asset.Btc));

            // Reliable test for buying.
            base.PretestPlaceOrderLimit("BTC_XRP".ToAssetPairRaw(), true, new Money(5000m, Asset.Xrp), new Money(0.00000010m, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("1c92173b-c6a2-4118-9ff0-b78bd775e0a8");
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            base.PretestPlaceWithdrawal(new WalletAddress("13zPXAsFofXXkczMg9bB6x1L9BWK9Yiawr"), new Money(0.10004911m, Asset.Btc));
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            base.PretestGetOrdersHistory();
        }

        [Fact]
        public override void TestGetOpenOrders()
        {
            base.PretestGetOpenOrders();
        }

        #endregion
    }
}