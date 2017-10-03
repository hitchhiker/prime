using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Plugins.Services.Bittrex;

namespace Prime.Tests
{
    [TestClass()]
    public class BittrexTests : ProviderDirectTestsBase
    {
        public BittrexTests()
        {
            Provider = Networks.I.Providers.OfType<BittrexProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestApiAsync()
        {
            await base.TestApiAsync();
        }

        [TestMethod]
        public override async Task TestGetDepositAddressesAsync()
        {
            await base.TestGetDepositAddressesAsync();
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
            await base.TestGetAssetPairsAsync();
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

        public override async Task GetLatestPriceAsync(IPublicPriceProvider provider)
        {
            var ctx = new PublicPriceContext(new AssetPair("BTC", "LTC"));

            try
            {
                var c = await provider.GetLatestPriceAsync(ctx);

                Assert.IsTrue(c != null);
                Assert.IsTrue(c.BaseAsset.Equals(ctx.Pair.Asset2));
                Assert.IsTrue(c.Price.Asset.Equals(ctx.Pair.Asset1));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}