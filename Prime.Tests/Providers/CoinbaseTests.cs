using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using plugins;
using Prime.Common;
using Prime.Plugins.Services.Coinbase;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class CoinbaseTests : ProviderDirectTestsBase
    {
        public CoinbaseTests()
        {
            Provider = Networks.I.Providers.OfType<CoinbaseProvider>().FirstProvider();
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
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

            base.TestGetAssetPairs(requiredPairs);
        }
        
        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("LTC_EUR".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            base.TestGetOhlc(context);
        }

        [TestMethod]
        public override void TestApi()
        {
            base.TestApi();
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Current);

            base.TestGetAddresses(context);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()));
            base.TestGetOrderBook(context);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 100);
            base.TestGetOrderBook(context);
        }
    }
}
