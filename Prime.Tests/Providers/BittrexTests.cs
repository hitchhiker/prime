using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
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
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestApi()
        {
            base.TestApi();
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
            var context = new WalletAddressAssetContext("BTC".ToAssetRaw(), UserContext.Current);

            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_XRP".ToAssetPairRaw(),
                "BTC_ETH".ToAssetPairRaw(),
                "BTC_ETC".ToAssetPairRaw(),
            };

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("BTC_LTC".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetPrices()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "BTC_LTC".ToAssetPairRaw(),
                "BTC_XRP".ToAssetPairRaw(),
                "BTC_BCC".ToAssetPairRaw()
            });

            base.TestGetPrices(context);
        }

        [TestMethod]
        public override void TestGetAssetPrices()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "BTC".ToAssetRaw()
            }, "WINGS".ToAssetRaw());

            base.TestGetAssetPrices(context);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "LTC".ToAssetRaw()));
            base.TestGetOrderBook(context);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "LTC".ToAssetRaw()), 100);
            base.TestGetOrderBook(context);
        }

        [TestMethod]
        public override void TestGetVolume()
        {
            var context = new VolumeContext("BTC_LTC".ToAssetPairRaw());

            base.TestGetVolume(context);
        }
    }
}