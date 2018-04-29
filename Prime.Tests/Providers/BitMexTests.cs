using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Core.Wallet.Withdrawal.Cancelation;
using Prime.Core.Wallet.Withdrawal.Confirmation;
using Prime.Core.Wallet.Withdrawal.History;
using Prime.Finance.Services.Services.BitMex;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitMexTests : ProviderDirectTestsBase
    {
        public BitMexTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();
        }

        #region Public

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("LTC_BTC".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            base.PretestGetOhlc(context);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
            };

            base.PretestGetAssetPairs(requiredPairs);
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
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Current);
            base.PretestGetAddresses(context);
        }

        [Fact]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.PretestGetAddressesForAsset(context);
        }
        
        [Fact]
        public override void TestGetWithdrawalHistory()
        {
            var context = new WithdrawalHistoryContext(UserContext.Current)
            {
                Asset = Asset.Btc
            };

            base.PretestGetWithdrawalHistory(context);
        }

        [Fact]
        public override void TestPlaceWithdrawal()
        {
            var token2fa = "249723";

            base.PretestPlaceWithdrawal(new WalletAddress("address"), new Money(1000, Asset.Btc), "Debug payment", new Money(0.004m, Asset.Btc), token2fa);
        }

        [Fact]
        public override void TestCancelWithdrawal()
        {
            var context = new WithdrawalCancelationContext()
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            base.PretestCancelWithdrawal(context);
        }

        [Fact]
        public override void TestConfirmWithdrawal()
        {
            var context = new WithdrawalConfirmationContext(UserContext.Current)
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            base.PretestConfirmWithdrawal(context);
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // Spend 10 USD to buy BTC at price 1 USD per BTC. In result 10 BTC will be bought.
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), true, new Money(10, Asset.Usd), new Money(1, Asset.Usd));

            // Sell 200 USD to buy 0.02 BTC at price 10000 USd.
            base.PretestPlaceOrderLimit("BTC_USD".ToAssetPairRaw(), false, new Money(200, Asset.Usd), new Money(10000, Asset.Usd));
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
            base.PretestGetTradeOrderStatus("ecb7f76a-5709-8a34-b4e7-f168d0a27cb8");
        }

        #endregion
    }
}