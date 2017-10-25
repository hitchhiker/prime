using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Core;
using Prime.Plugins.Services.Binance;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class BinanceTests : ProviderDirectTestsBase
    {
        public BinanceTests()
        {
            Provider = Networks.I.Providers.OfType<BinanceProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetPairPriceAsync()
        {
            PublicPairPriceContext = new PublicPairPriceContext(new AssetPair("ETH", "BTC"));
            await base.TestGetPairPriceAsync();

            PublicPairPriceContext = new PublicPairPriceContext(new AssetPair("STRAT", "ETH"));
            await base.TestGetPairPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            PublicAssetPricesContext = new PublicAssetPricesContext(new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "BTC".ToAssetRaw()
            }, "BNT".ToAssetRaw());

            await base.TestGetAssetPricesAsync();

            PublicAssetPricesContext = new PublicAssetPricesContext(new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "BTC".ToAssetRaw()
            }, "SALT".ToAssetRaw());

            await base.TestGetAssetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetPairsPricesAsync()
        {
            PublicPairsPricesContext = new PublicPairsPricesContext(new List<AssetPair>()
            {
                "BNT_ETH".ToAssetPairRaw(),
                "BNT_BTC".ToAssetPairRaw(),
                "SALT_ETH".ToAssetPairRaw(),
                "SALT_BTC".ToAssetPairRaw()
            });

            await base.TestGetPairsPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair("BNT".ToAssetRaw(), "BTC".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair("BNT".ToAssetRaw(), "BTC".ToAssetRaw()), 20);
            await base.TestGetOrderBookAsync();
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync();
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync();
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            OhlcContext = new OhlcContext(new AssetPair("BNT", "BTC"), TimeResolution.Minute,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute), null);
            await base.TestGetOhlcAsync();
        }
    }
}
