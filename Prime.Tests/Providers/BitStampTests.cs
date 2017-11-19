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
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
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
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.TestGetAddressesForAsset(context);
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
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

            base.TestGetAssetPairs(requiredPairs);
        }

        [TestMethod]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
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
