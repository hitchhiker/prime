using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using plugins;
using Prime.Core;

namespace Prime.TestConsole
{
    public partial class Program 
    {
        public class BitMexTests
        {
            public void GetOhlcData()
            {
                var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

                // BUG: what is the purpose of TimeRange.EveryDayTillNow parameter?
                var ohlcContext = new OhlcContext(new AssetPair("XBt", "USD"), TimeResolution.Minute, TimeRange.EveryDayTillNow, null);

                var ohlc = AsyncContext.Run(() => provider.GetOhlcAsync(ohlcContext));

                try
                {
                    foreach (var data in ohlc)
                    {
                        Console.WriteLine($"{data.DateTimeUtc}: {data.High}-{data.Low} {data.Open}-{data.Close}");
                    }
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
