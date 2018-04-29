using System.Collections.Generic;
using System.Linq;
using Prime.Core;
using Prime.Finance.Services.Services.ShapeShift;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class ShapeShiftTests : ProviderDirectTestsBase
    {
        public ShapeShiftTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<ShapeShiftProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            base.PretestGetAssetPairs(new AssetPairs()
            {
                "BTC_ETH".ToAssetPairRaw(),
                "SWT_XRP".ToAssetPairRaw(),
                "BCH_ETH".ToAssetPairRaw()
            });
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_ETH".ToAssetPairRaw(),
                "SWT_XRP".ToAssetPairRaw(),
                "BCH_ETH".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }
    }
}
