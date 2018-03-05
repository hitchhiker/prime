using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;
using Prime.Plugins.Services.BitMex;
using Xunit;

namespace Prime.Tests.Providers
{
    public class BitMexTests : ProviderDirectTestsBase
    {
        public BitMexTests()
        {
            Provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();
        }

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
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
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
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
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

        // [Fact]
        public override void TestCancelWithdrawal()
        {
            var context = new WithdrawalCancelationContext()
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            base.PretestCancelWithdrawal(context);
        }

        // [Fact]
        public override void TestConfirmWithdrawal()
        {
            var context = new WithdrawalConfirmationContext(UserContext.Current)
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            base.PretestConfirmWithdrawal(context);
        }
    }
}