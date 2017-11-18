using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx;
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
        public void TestAllAssetPairsPrices()
        {
            var pairs = AsyncContext.Run(() => (IAssetPairsProvider) Provider).GetAssetPairsAsync(new NetworkProviderContext()).Result;

            var prices = AsyncContext.Run(() => (IDELETEPublicPricesProvider) Provider).GetPricesAsync(new PublicPricesContext(pairs.ToList())).Result;
                ;

            Assert.IsTrue(!prices.MissedPairs.Any());
            Assert.IsTrue(pairs.Count == prices.MarketPrices.Count);
        }

        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [TestMethod]
        public override void TestGetPrice()
        {
            var context = new PublicPriceContext("LTC_BTC".ToAssetPairRaw());
            base.TestGetPrice(context, true);

            context = new PublicPriceContext("BCH_EUR".ToAssetPairRaw());
            base.TestGetPrice(context, false);
        }

        [TestMethod]
        public override void TestGetAssetPrices()
        {
            var context = new PublicAssetPricesContext(new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "LTC".ToAssetRaw(),
                "XRP".ToAssetRaw()
            }, "BTC".ToAssetRaw());

            base.TestGetAssetPrices(context);
        }

        [TestMethod]
        public override void TestGetPrices()
        {
            var context = new PublicPricesContext(new List<AssetPair>()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            });

            base.TestGetPrices(context);

            context = new PublicPricesContext(new List<AssetPair>()
            {
                "BCH_EUR".ToAssetPairRaw(),
                "BCH_USD".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw(),
                "EOS_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw(),
                "GNO_ETH".ToAssetPairRaw(),
                "GNO_BTC".ToAssetPairRaw(),
                "DASH_EUR".ToAssetPairRaw(),
                "DASH_USD".ToAssetPairRaw(),
                "DASH_BTC".ToAssetPairRaw()
            });

            base.TestGetPrices(context);
        }

        [TestMethod]
        public override void TestGetAddresses()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.

            var context = new WalletAddressContext(UserContext.Current);
            base.TestGetAddresses(context);
        }

        [TestMethod]
        public override void TestGetAddressesForAsset()
        {
            // BUG: EFunding:Too many addresses. Should investigate that.
            var context = new WalletAddressAssetContext("MLN".ToAssetRaw(), UserContext.Current);
            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("ETC_ETH".ToAssetPairRaw(), TimeResolution.Minute, TimeRange.EveryDayTillNow);
            base.TestGetOhlc(context);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var pairs = new AssetPairs()
            {
                "ETC_ETH".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
            };

            base.TestGetAssetPairs(pairs);
        }

        [TestMethod]
        public override void TestApi()
        {
            base.TestApi();
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
