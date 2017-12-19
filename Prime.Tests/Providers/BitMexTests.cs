using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
using Prime.Common.Wallet.Withdrawal.History;
using Prime.Plugins.Services.BitMex;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BitMexTests : ProviderDirectTestsBase
    {
        public BitMexTests()
        {
            Provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
            };

            base.TestGetPricing(pairs, false);
        }

        [TestMethod]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [TestMethod]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [TestMethod]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("LTC_BTC".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            base.TestGetOhlc(context);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Current);
            base.TestGetAddresses(context);
        }

        [TestMethod]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            base.TestGetOrderBook(new AssetPair(Asset.Btc, "USD".ToAssetRaw()), false);
        }

        [TestMethod]
        public override void TestGetWithdrawalHistory()
        {
            var context = new WithdrawalHistoryContext(UserContext.Current)
            {
                Asset = Asset.Btc
            };

            base.TestGetWithdrawalHistory(context);
        }

        // [TestMethod]
        public override void TestPlaceWithdrawalExtended()
        {
            var token2fa = "249723";

            var context = new WithdrawalPlacementContextExtended(UserContext.Current)
            {
                Amount = new Money(0.001m, Asset.Btc),
                Address = null,
                AuthenticationToken = token2fa,
                CustomFee = new Money(0.004m, Asset.Btc),
                Description = "Debug payment"
            };

            base.TestPlaceWithdrawalExtended(context);
        }

        // [TestMethod]
        public override void TestCancelWithdrawal()
        {
            var context = new WithdrawalCancelationContext()
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            base.TestCancelWithdrawal(context);
        }

        // [TestMethod]
        public override void TestConfirmWithdrawal()
        {
            var context = new WithdrawalConfirmationContext(UserContext.Current)
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            base.TestConfirmWithdrawal(context);
        }
    }
}