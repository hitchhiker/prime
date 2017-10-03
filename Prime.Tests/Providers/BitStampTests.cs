using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Core;
using Prime.Core.Wallet;
using Prime.Plugins.Services.BitStamp;

namespace Prime.Tests
{
    [TestClass()]
    public class BitStampTests : ProviderDirectTestsBase
    {
        public BitStampTests()
        {
            Provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();
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
