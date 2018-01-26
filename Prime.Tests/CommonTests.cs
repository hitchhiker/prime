using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prime.Common;

namespace Prime.Tests
{
    [TestClass]
    public class CommonTests
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void TestReversePricingStatistics()
        {
            var stats = new PriceStatistics(new Network("Test network"), Asset.Usd, 1100m, 900m, 800m, 1200m);
            Trace.WriteLine($"{stats}");
            var statsReversed = stats.Reverse(Asset.Btc);
            Trace.WriteLine($"{statsReversed}");
        }
    }
}
