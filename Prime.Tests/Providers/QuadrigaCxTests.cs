using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.BitStamp;
using Prime.Plugins.Services.QuadrigaCX;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class QuadrigaCxTests : ProviderDirectTestsBase
    {
        public QuadrigaCxTests()
        {
            Provider = Networks.I.Providers.OfType<QuadrigaCxProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("eth_btc".ToAssetPairRaw());
            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }
    }
}
