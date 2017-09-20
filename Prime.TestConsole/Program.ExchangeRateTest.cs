using System;
using System.Threading;
using Prime.Core;
using Prime.Core.Exchange.Rates;
using Prime.Utility;

namespace TestConsole
{
    public partial class Program
    {
        public class ExchangeRateTest
        {
            public void Test()
            {
                DefaultMessenger.I.Default.Register<NewLogMessage>(this, m =>
                {
                    Console.WriteLine(m.Message);
                });

                var exch = ExchangeRatesCoordinator.I;
                exch.AddRequest(new AssetPair("btc", "usd"));
                exch.AddRequest(new AssetPair("usd", "btc"));

                /*exch.Register<ExchangeRateCollected>(this, m =>
                {
                    Console.WriteLine(m.UtcCreated.ToLongTimeString() + ": " + m.Pair + " = " + m.Price.ToString());
                });*/

                exch.Register<ExchangeRatesUpdatedMessage>(this, m =>
                {
                    foreach (var r in exch.Results())
                        Console.WriteLine(r.UtcCreated.ToLongTimeString() + ": " + r.Pair + " = " + r.Price.ToString());
                });

                do
                {
                    Thread.Sleep(1);
                } while (1 == 1);
            }
        }
    }
}