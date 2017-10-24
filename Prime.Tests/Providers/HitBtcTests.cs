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
            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetAddressesForAssetAsync()
        {
            WalletAddressAssetContext = new WalletAddressAssetContext("BCC".ToAssetRaw(), false, UserContext.Current);
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
    }
}
