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
        public override void TestPublicApi()
        {
            base.TestPublicApi();
        }

        [TestMethod]
        public override void TestGetAssetPairs()
        {
            var context = new AssetPairs()
            {
                "BTC_KRW".ToAssetPairRaw(),
                "ETC_KRW".ToAssetPairRaw(),
                "ETH_KRW".ToAssetPairRaw(),
                "XRP_KRW".ToAssetPairRaw(),
            };

            base.TestGetAssetPairs(context);
        }

        [TestMethod]
        public override void TestGetOrderBook()
        {
            var context = new OrderBookContext(new AssetPair("BTC", "KRW"), 10);
            base.TestGetOrderBook(context);

            context = new OrderBookContext(new AssetPair("BTC", "KRW"));
            base.TestGetOrderBook(context);
        }
    }
}
