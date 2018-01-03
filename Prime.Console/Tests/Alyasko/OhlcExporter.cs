using System;
using System.IO;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Binance;
using Prime.Plugins.Services.BitMex;

namespace Prime.Console.Tests.Alyasko
{
    public class OhlcExporter : ITestBase
    {
        public void Go()
        {
            var ohlcs = Networks.I.Providers.OfType<IOhlcProvider>().ToArray();
            foreach (var provider in ohlcs)
            {
                System.Console.WriteLine($"Name: {provider.Network.Name}");
            }

            var bitmex = Networks.I.Providers.OfType<IOhlcProvider>().First(x => x.Network.Name.Equals("bitfinex", StringComparison.InvariantCultureIgnoreCase));

            int daysMinus = 120;

            var ctx = new OhlcContext("XRP_USD".ToAssetPairRaw(), TimeResolution.Hour,
                new TimeRange(DateTime.UtcNow.AddDays(-daysMinus*2), DateTime.UtcNow.AddDays(-daysMinus),
                    TimeResolution.Hour));

            var r = bitmex.GetOhlcAsync(ctx).Result;

            var contents = string.Join("\r\n", r.Select(x => $"{x.DateTimeUtc} - {(x.High + x.Low) / 2}"));

            File.WriteAllText(@"ohlc-xrp-usd-2.txt", contents);
        }
    }
}
