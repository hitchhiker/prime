using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Core; using Prime.Finance;
using Prime.Finance.Services.Services.Whaleclub;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public class WhaleclubTests : ProviderDirectTestsBase
    {
        public WhaleclubTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<WhaleclubProvider>().FirstProvider();
        }

        [Obsolete("Public methods require key.")]
        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Obsolete("Public methods require key.")]
        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DASH_USD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Obsolete("Public methods require key.")]
        [Fact]
        public override void TestGetAssetPairs()
        { 
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}
