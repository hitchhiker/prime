using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Core;
using Prime.Plugins.Services.BitMex;

namespace Prime.TestConsole
{
    public partial class Program 
    {
        public class BitMexTests
        {
            public void GetOhlcData()
            {
                var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

                var ohlcContext = new OhlcContext(new AssetPair("BTC", "USD"), TimeResolution.Hour, new TimeRange(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow, TimeResolution.Minute), null);

                try
                {
                    var ohlc = AsyncContext.Run(() => provider.GetOhlcAsync(ohlcContext));

                    foreach (var data in ohlc)
                    {
                        Console.WriteLine($"{data.DateTimeUtc}: {data.High}-{data.Low} {data.Open}-{data.Close}");
                    }

                    Console.WriteLine($"Entries count: {ohlc.Count}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }
    }
}
