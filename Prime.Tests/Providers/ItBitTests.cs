using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.ItBit;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class ItBitTests : ProviderDirectTestsBase
    {
        public ItBitTests()
        {
            Provider = Networks.I.Providers.OfType<ItBitProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            RequiredAssetPairs = new AssetPairs(new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_SGD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
            });

            await base.TestGetAssetPairsAsync();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            await base.TestGetPriceAsync();
        }

        [TestMethod]
        public override async Task TestGetVolumeAsync()
        {
            var ctx = new VolumeContext()
            {
                Pair = "BTC_USD".ToAssetPairRaw()
            };
            GetVolumeFunc = () => ((ItBitProvider)Provider).GetVolumeAsync(ctx);

            await base.TestGetVolumeAsync();
        }
    }
}
