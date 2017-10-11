using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Plugins.Services.Bittrex;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BittrexTests : ProviderDirectTestsBase
    {
        // OHLC data is not provided by API.

        public BittrexTests()
        {
            Provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync();
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
        public override async Task TestGetAssetPairsAsync()
        {
            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync();
        }

        [TestMethod]
        public override async Task TestGetLatestPriceAsync()
        {
            PublicPriceContext = new PublicPriceContext(new AssetPair("BTC", "LTC"));
            await base.TestGetLatestPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "LTC".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "LTC".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync();
        }

        [TestMethod]
        public override async Task TestGetLatestPricesAsync()
        {
            PublicPricesContext = new PublicPricesContext(Asset.Btc, new List<Asset>()
            {
                "LTC".ToAssetRaw(),
                "XRP".ToAssetRaw()
            });
            await base.TestGetLatestPricesAsync();
        }
    }
}