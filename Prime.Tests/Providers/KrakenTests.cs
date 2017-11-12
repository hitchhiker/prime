using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Kraken;
using AssetPair = Prime.Common.AssetPair;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class KrakenTests : ProviderDirectTestsBase
    {
        public KrakenTests()
        {
            Provider = Networks.I.Providers.OfType<KrakenProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync();
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
            PublicAssetPricesContext = new PublicAssetPricesContext(new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "LTC".ToAssetRaw(),
                "XRP".ToAssetRaw()
            }, "BTC".ToAssetRaw());

            await base.TestGetAssetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            PublicPricesContext = new PublicPricesContext(new List<AssetPair>()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            });

            await base.TestGetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.
            await base.TestGetAddressesAsync();
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.
            WalletAddressAssetContext = new WalletAddressAssetContext("MLN".ToAssetRaw(), UserContext.Current);
            await base.TestGetAddressesForAssetAsync();
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            await base.TestGetOhlcAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync();
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            await base.TestGetVolumeAsync();
        }
    }
}
