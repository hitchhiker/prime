using System;
using System.Linq;
using System.Threading.Tasks;
using KrakenApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Plugins.Services.Kraken;

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
        public override async Task TestGetBalancesAsync()
        {
            await base.TestGetBalancesAsync();
        }

        [TestMethod]
        public override async Task TestGetLatestPriceAsync()
        {
            await base.TestGetLatestPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetLatestPricesAsync()
        {
            await base.TestGetLatestPricesAsync();
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

        public override async Task GetAddressesForAssetAsync(IDepositService provider)
        {
            var asset = "XDG".ToAsset(provider);

            var ctx = new WalletAddressAssetContext(asset, false, UserContext.Current);

            try
            {
                var r = await provider.GetAddressesForAssetAsync(ctx);
                Assert.IsTrue(r != null);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public override async Task TestGetOhlcAsync()
        {
            await base.TestGetOhlcAsync();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookLiveAsync()
        {
            await base.TestGetOrderBookLiveAsync();
        }

        [TestMethod]
        public override async Task TestGetOrderBookHistoryAsync()
        {
            await base.TestGetOrderBookHistoryAsync();
        }
    }
}
