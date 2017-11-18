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
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesAsync()
        {
            var context = new WalletAddressContext(UserContext.Current);

            await base.TestGetAddressesAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            await base.TestGetAddressesForAssetAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
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

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_USD".ToAssetPairRaw());
            await base.TestGetPrice(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            var context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()));
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);

            context = new OrderBookContext(new AssetPair("BTC".ToAssetRaw(), "USD".ToAssetRaw()), 100);
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);
        }
    }
}
