using System;
using System.Diagnostics;
using System.Threading;
using LiteDB;
using Prime.Base;
using Prime.Core;
using Prime.Finance;
using Xunit;
using Xunit.Abstractions;

namespace Prime.Tests
{
    public class CommonTests
    {
        private ITestOutputHelper _outputHelper;

        public CommonTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void TestReverseOrderBook()
        {
            Trace.WriteLine("BTC:USD test");
            var r = OrderBookRecord.CreateInternal(OrderType.Ask, new Money(10000, Asset.Usd), new Money(20000, Asset.Usd));
            PrintOrderBook(r);
            var rReversed = r.Reverse(Asset.Btc);
            PrintOrderBook(rReversed);

            var r2 = OrderBookRecord.CreateInternal(OrderType.Bid, new Money(0.0001m, Asset.Btc), new Money(0.0002m, Asset.Btc));
            PrintOrderBook(r2);
            var r2Reversed = r2.Reverse(Asset.Usd);
            PrintOrderBook(r2Reversed);

            Trace.WriteLine("\nCAR:USD test");
            var rCar = OrderBookRecord.CreateInternal(OrderType.Ask, new Money(5000, Asset.Usd), new Money(10000, Asset.Usd));
            PrintOrderBook(rCar);
            var rCarReversed = rCar.Reverse("CAR".ToAssetRaw());
            PrintOrderBook(rCarReversed);

            void PrintOrderBook(OrderBookRecord obr)
            {
                Trace.WriteLine($"{obr}, Price: {obr.Price.Display}, Volume: {obr.Volume.Display}");
            }
        }

        [Fact]
        public void TestReversePricing()
        {
            var n = new Network("Test network");
            var p = new MarketPrice(n, "BTC_USD".ToAssetPairRaw(), 10_000m)
            {
                PriceStatistics = new PriceStatistics(n, Asset.Usd, 1100m, 900m, 800m, 1200m)
            };
            Trace.WriteLine($"{p}");
            Trace.WriteLine($"{p.PriceStatistics}");

            var pReversed = p.Reversed;

            Trace.WriteLine($"{pReversed}");
            Trace.WriteLine($"{pReversed.PriceStatistics}");
        }

        [Fact]
        public void TestReversePricingStatistics()
        {
            var stats = new PriceStatistics(new Network("Test network"), Asset.Usd, 1100m, 900m, 800m, 1200m);
            Trace.WriteLine($"{stats}");
            var statsReversed = stats.Reverse(Asset.Btc);
            Trace.WriteLine($"{statsReversed}");
        }

        [Fact]
        public void ObjectIdTests()
        {
            _outputHelper.WriteLine("Object Id test");

            _outputHelper.WriteLine($"{ObjectId.NewObjectId()}");
        }

        [Fact]
        public void EpochTests()
        {
            PrintNonce();
            Thread.Sleep(1000);
            PrintNonce();
            Thread.Sleep(1000);
            PrintNonce();
            Thread.Sleep(1000);

            void PrintNonce()
            {
                var nonce = (DateTime.UtcNow.Ticks - new DateTime(2000, 01, 1).Ticks) / 1000_0000; // 1s
                _outputHelper.WriteLine($"Nonce: {nonce}");
            }
        }

        [Fact]
        public void CacheItemTest()
        {
            var item = new CacheItem<int>(TimeSpan.FromSeconds(3), 19);
            var fresh = item.IsFresh;
        }
    }
}
