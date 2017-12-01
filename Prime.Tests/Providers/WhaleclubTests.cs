using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Whaleclub;

namespace Prime.Tests.Providers
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    [TestClass]
    public class WhaleclubTests : ProviderDirectTestsBase
    {
        public WhaleclubTests()
        {
            Provider = Networks.I.Providers.OfType<WhaleclubProvider>().FirstProvider();
        }

        [Obsolete("Public methods require key.")]
        [TestMethod]
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [Obsolete("Public methods require key.")]
        [TestMethod]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DASH_USD".ToAssetPairRaw()
            };

            base.TestGetPricing(pairs, false);
        }

        [Obsolete("Public methods require key.")]
        [TestMethod]
        public override void TestGetAssetPairs()
        { 
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw()
            };

            base.TestGetAssetPairs(requiredPairs);
        }
    }
}
