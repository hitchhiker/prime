using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Plugins.Services.BitMex;

namespace Prime.Tests
{
    [TestClass()]
    public class BitMexTests : ProviderDirectTestsBase
    {
        public BitMexTests()
        {
            Provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestApisAsync()
        {
            await base.TestApisAsync();
        }

        [TestMethod]
        public override async Task TestGetDepositAddressesAsync()
        {
            await base.TestGetDepositAddressesAsync();
        }
    }
}