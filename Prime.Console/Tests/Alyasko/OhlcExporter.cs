using System;
using System.IO;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.BitMex;

namespace Prime.Console.Tests.Alyasko
{
    public class OhlcExporter : ITestBase
    {
        public void Go()
        {
            var ohlcs = Networks.I.OhlcProviders.ToArray();
            foreach (var provider in ohlcs)
            {
                System.Console.WriteLine($"Name: {provider.Network.Name}");
            }

            var bitmex = Networks.I.Providers.OfType<BitMexProvider>().First();

            var r = bitmex.GetOhlcAsync(new OhlcContext("BTC_USD".ToAssetPairRaw(), TimeResolution.Hour,
                new TimeRange(DateTime.UtcNow.AddDays(-60), DateTime.UtcNow.AddDays(-40), TimeResolution.Hour))).Result;

            var contents = string.Join("\r\n", r.Select(x => $"{x.DateTimeUtc} - {(x.High + x.Low) / 2}"));

            File.WriteAllText(@"C:\Users\Alexander\Desktop\ohlc btc-usd 1h 3.txt", contents);
        }
    }
}
