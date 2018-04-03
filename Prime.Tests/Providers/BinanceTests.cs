using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Plugins.Services.Binance;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests.Providers
{
    public class BinanceTests : ProviderDirectTestsBase 
    {
        public BinanceTests(ITestOutputHelper outputWriter) : base(outputWriter)
        {
            Provider = Networks.I.Providers.OfType<BinanceProvider>().FirstProvider();
        }

        #region Private

        [Fact]
        public override void TestApiPrivate()
        {
            base.TestApiPrivate();
        }

        [Fact]
        public override void TestGetBalances()
        {
            base.TestGetBalances();
        }

        [Fact]
        public override void TestPlaceOrderLimit()
        {
            // Reliable buy order test.
            base.PretestPlaceOrderLimit("XRP_BTC".ToAssetPairRaw(), true, new Money(100000m, Asset.Xrp), new Money(0.00000001m, Asset.Btc));

            // Reliable sell order test.
            base.PretestPlaceOrderLimit("XRP_BTC".ToAssetPairRaw(), false, new Money(1m, Asset.Xrp), new Money(1m, Asset.Btc));
        }

        [Fact]
        public override void TestGetTradeOrderStatus()
        {
            base.PretestGetTradeOrderStatus("34009816", "TRX_BTC".ToAssetPairRaw());
        }
        
        [Fact]
        public override void TestPlaceWithdrawal()
        {
            base.PretestPlaceWithdrawal(new WalletAddress("rLW9gnQo7BQhU6igk5keqYnH3TVrCxGRzm"), new Money(220, Asset.Xrp), "3299088538");
        }

        [Fact]
        public override void TestGetOrdersHistory()
        {
            base.PretestGetOrdersHistory("TRX_BTC".ToAssetPairRaw());
        }

        [Fact]
        public override void TestGetOpenOrders()
        {
            base.PretestGetOpenOrders();
        }

        public async Task TestGetDepositHistory()
        {
            var context = new NetworkProviderPrivateContext(UserContext.Current);
            var binanceProvider = new BinanceProvider();

            await binanceProvider.GetDepositHistoryAsync(context).ConfigureAwait(false);
        }

        #endregion

        #region Public

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
                "BCH_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "BNT_BTC".ToAssetPairRaw(),
                "SALT_ETH".ToAssetPairRaw(),
                "SALT_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "NEO_BTC".ToAssetPairRaw(),
                "LTC_BTC".ToAssetPairRaw(),
                "ETH_BTC".ToAssetPairRaw(),
                "NEO_ETH".ToAssetPairRaw(),
                "IOTA_BTC".ToAssetPairRaw(),
                "ETC_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("BTC_USDT".ToAssetPairRaw(), false);
        }

        [Fact]
        public override void TestGetOhlc()
        {
            var context = new OhlcContext("BTC_USDT".ToAssetPairRaw(), TimeResolution.Minute,
                new TimeRange(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, TimeResolution.Minute));
            base.PretestGetOhlc(context);
        }

        #endregion
    }
}
