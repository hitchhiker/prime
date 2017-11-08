using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Common.Wallet.Withdrawal.Cancelation;
using Prime.Common.Wallet.Withdrawal.Confirmation;
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
            await base.TestApiAsync();
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            await base.TestGetOhlcAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            PublicPriceContext = new PublicPriceContext("LTC_BTC".ToAssetPairRaw());
            await base.TestGetPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            PublicAssetPricesContext = new PublicAssetPricesContext(new List<Asset>()
            {
                "LTC".ToAssetRaw(),
                "ETH".ToAssetRaw(),
                "FCT".ToAssetRaw()
            }, Asset.Btc);
            await base.TestGetAssetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            PublicPricesContext = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DAO_ETH".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "FCT_BTC".ToAssetPairRaw()
            });
            await base.TestGetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync();
        }

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            await base.TestGetAddressesAsync();
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            await base.TestGetAddressesForAssetAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair(Asset.Btc, "USD".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair(Asset.Btc, "USD".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync();
        }

        [TestMethod]
        public override async Task TestGetWithdrawalHistoryAsync()
        {
            await base.TestGetWithdrawalHistoryAsync();
        }

        [TestMethod]
        public override async Task TestPlaceWithdrawalExtendedAsync()
        {
            var token2fa = "249723";

            WithdrawalPlacementContextExtended = new WithdrawalPlacementContextExtended(UserContext.Current)
            {
                Price = new Money(0.001m, Asset.Btc),
                Address = "2NBMEXqYb3FXiui3ZZA5TzHV85LqN7yFDgP",
                AuthenticationToken = token2fa,
                CustomFee = new Money(0.004m, Asset.Btc),
                Description = "Debug payment"
            };

            await base.TestPlaceWithdrawalExtendedAsync();
        }

        [TestMethod]
        public override async Task TestCancelWithdrawalAsync()
        {
            WithdrawalCancelationContext = new WithdrawalCancelationContext()
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            await base.TestCancelWithdrawalAsync();
        }

        [TestMethod]
        public override async Task TestConfirmWithdrawalAsync()
        {
            WithdrawalConfirmationContext = new WithdrawalConfirmationContext(UserContext.Current)
            {
                WithdrawalRemoteId = "41022240-e2bd-80d4-3e23-ad4c872bd43a"
            };

            await base.TestConfirmWithdrawalAsync();
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            var ctx = new VolumeContext()
            {
                Pair = "BTC_USD".ToAssetPairRaw()
            };
            GetVolumeFunc = () => ((BitMexProvider) Provider).GetVolumeAsync(ctx);

            await base.TestGetVolumeAsync();
        }
    }
}