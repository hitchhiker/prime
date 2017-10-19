using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public override async Task TestGetLatestPriceAsync()
        {
            PublicPriceContext = new PublicPriceContext(new AssetPair("ETH", "BTC"));
            await base.TestGetLatestPriceAsync();

            PublicPriceContext = new PublicPriceContext(new AssetPair("STRAT", "ETH"));
            await base.TestGetLatestPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetLatestPricesAsync()
        {
            PublicPricesContext = new PublicPricesContext("BNT".ToAssetRaw(), new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "BTC".ToAssetRaw()
            });

            await base.TestGetLatestPricesAsync();

            PublicPricesContext = new PublicPricesContext("SALT".ToAssetRaw(), new List<Asset>()
            {
                "ETH".ToAssetRaw(),
                "BTC".ToAssetRaw()
            });

            await base.TestGetLatestPricesAsync();
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
    }
}
