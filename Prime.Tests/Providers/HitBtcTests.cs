using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.HitBtc;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class HitBtcTests : ProviderDirectTestsBase
    {
        public HitBtcTests()
        {
            Provider = Networks.I.Providers.OfType<HitBtcProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs()
            {
                "EUR_BTC".ToAssetPairRaw(),
                "USD_BTC".ToAssetPairRaw(),
                "BTC_DOGE".ToAssetPairRaw(),
                "EUR_LTC".ToAssetPairRaw(),
                "USD_ETH".ToAssetPairRaw(),
                "ETH_DASH".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            WalletAddressAssetContext = new WalletAddressAssetContext("BCC".ToAssetRaw(), UserContext.Current);
            await base.TestGetAddressesForAssetAsync();
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
        public override async Task TestGetPriceAsync()
        {
            await base.TestGetPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPricesAsync()
        {
            await base.TestGetAssetPricesAsync();
        }

        [TestMethod]
        public override async Task TestGetPricesAsync()
        {
            await base.TestGetPricesAsync();
        }
    }
}
