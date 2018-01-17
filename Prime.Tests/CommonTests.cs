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
        public void TestReversing()
        {
            var r = OrderBookRecord.CreateInternal(OrderType.Ask, new Money(13000, Asset.Usd), new Money(10, Asset.Btc));
            PrintOrderBook(r);
            var rReversed = r.Reverse(Asset.Btc);
            PrintOrderBook(rReversed);

            void PrintOrderBook(OrderBookRecord obr)
            {
                Trace.WriteLine($"Original: {obr}, Price: {obr.Price.Display}, Volume: {obr.Volume.Display}");
            }
        }
    }
}
