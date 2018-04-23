using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Finance.Services.Services.BitStamp;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BitStampTests : ProviderDirectTestsBase
    {
        // OHLC data is not provided by API.
        
        public BitStampTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BitStampProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "EUR_USD".ToAssetPairRaw(),
                "XRP_USD".ToAssetPairRaw(),
                "XRP_EUR".ToAssetPairRaw(),
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestApiPrivate()
        {
           base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetAddresses()
        {
            var context = new WalletAddressContext(UserContext.Current);

            base.PretestGetAddresses(context);
        }

        [Fact]
        public override void TestGetAddressesForAsset()
        {
            var context = new WalletAddressAssetContext(Asset.Btc, UserContext.Current);

            base.PretestGetAddressesForAsset(context);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw(),
                "BTC_EUR".ToAssetPairRaw(),
                "EUR_USD".ToAssetPairRaw(),
                "XRP_USD".ToAssetPairRaw(),
                "XRP_EUR".ToAssetPairRaw(),
                "XRP_BTC".ToAssetPairRaw(),
                "LTC_USD".ToAssetPairRaw(),
                "LTC_EUR".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_USD".ToAssetPairRaw(),
                "ETH_EUR".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw()
            };  

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USD".ToAssetPairRaw(), false);
        }
    }
}
