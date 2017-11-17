using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;
using Prime.Plugins.Services.Korbit;

namespace Prime.Tests.Providers
{
    [TestClass]
    public class KorbitTests : ProviderDirectTestsBase
    {
        public KorbitTests()
        {
            Provider = Networks.I.Providers.OfType<KorbitProvider>().FirstProvider();
        }

        [TestMethod]
        public override async Task TestPublicApiAsync()
        {
            await base.TestPublicApiAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetAssetPairsAsync()
        {
            var context = new AssetPairs()
            {
                "BTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
            };

            await base.TestGetAssetPairsAsync(context).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetPriceAsync()
        {
            var context = new PublicPriceContext("BTC_KRW".ToAssetPairRaw());

            await base.TestGetPriceAsync(context, false).ConfigureAwait(false);
        }

        [TestMethod]
        public override async Task TestGetOrderBookAsync()
        {
            var context = new OrderBookContext(new AssetPair("BTC", "KRW"), 10);
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);

            context = new OrderBookContext(new AssetPair("BTC", "KRW"));
            await base.TestGetOrderBookAsync(context).ConfigureAwait(false);
        }
    }
}
