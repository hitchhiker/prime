using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.TheRockTrading;

namespace Prime.Tests.Providers
{
    [TestClass()]
    public class TheRockTradingTests : ProviderDirectTestsBase
    {
        public TheRockTradingTests()
        {
            Provider = Networks.I.Providers.OfType<TheRockTradingProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_EUR".ToAssetPairRaw());
            await base.TestGetPrice(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BTC_XRP".ToAssetPairRaw(),
                "EUR_XRP".ToAssetPairRaw(),
                "USD_XRP".ToAssetPairRaw(),
                "PPC_EUR".ToAssetPairRaw(),
                "PPC_BTC".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "ZEC_BTC".ToAssetPairRaw(),
                "ZEC_EUR".ToAssetPairRaw(),
                "BCH_BTC".ToAssetPairRaw()
            };

            await base.TestGetAssetPairsAsync(requiredPairs).ConfigureAwait(false);
        }
    }
}
