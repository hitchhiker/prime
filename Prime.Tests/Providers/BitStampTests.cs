using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitStamp;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class BitStampTests : ProviderDirectTestsBase
    {
        // OHLC data is not provided by API.
        
        public BitStampTests()
        {
            Provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();
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
            RequiredAssetPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "EUR_USD".ToAssetPairRaw(),
                "XRP_USD".ToAssetPairRaw(),
                "XRP_EUR".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };  

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            await base.TestGetPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()));
            await base.TestGetOrderBookAsync();

            OrderBookContext = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync();
        }
    }
}
