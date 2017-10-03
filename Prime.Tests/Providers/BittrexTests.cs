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