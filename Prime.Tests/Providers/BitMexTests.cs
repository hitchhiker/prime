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
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            var context = new OhlcContext("LTC_BTC".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            await base.TestGetOhlcAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("LTC_BTC".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, true).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "LTC".ToAssetRaw(),
                "ETH".ToAssetRaw(),
                "FCT".ToAssetRaw()
            }, Asset.Btc);

            await base.TestGetAssetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DAO_ETH".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "FCT_BTC".ToAssetPairRaw()
            });
            await base.TestGetPricesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            var context = new WalletAddressContext(UserContext.Current);
            await base.TestGetAddressesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            await base.TestGetAddressesForAssetAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            var context = new OrderBookContext(new AssetPair(Asset.Btc, "USD".ToAssetRaw()));
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);

            context = new OrderBookContext(new AssetPair(Asset.Btc, "USD".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override Task TestGetWithdrawalHistoryAsync()
        {
            var context = new WithdrawalHistoryContext(UserContext.Current);

            await base.TestGetWithdrawalHistoryAsync(context).ConfigureAwait(false);
        }

        // [TestMethod]
        public override async Task TestPlaceWithdrawalExtendedAsync()
        {
            var token2fa = "249723";

            var context = new WithdrawalPlacementContextExtended(UserContext.Current)
            {
                Price = new Money(0.001m, Asset.Btc),
                Address = "2NBMEXqYb3FXiui3ZZA5TzHV85LqN7yFDgP",
                AuthenticationToken = token2fa,
                CustomFee = new Money(0.004m, Asset.Btc),
                Description = "Debug payment"
            };

            await base.TestPlaceWithdrawalExtendedAsync(context).ConfigureAwait(false);
        }

        // [TestMethod]
        public override async Task TestCancelWithdrawalAsync()
        {
            var context = new WithdrawalCancelationContext()
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            await base.TestCancelWithdrawalAsync(context).ConfigureAwait(false);
        }

        // [TestMethod]
        public override async Task TestConfirmWithdrawalAsync()
        {
            var context = new WithdrawalConfirmationContext(UserContext.Current)
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            await base.TestConfirmWithdrawalAsync(context).ConfigureAwait(false);
        }
    }
}