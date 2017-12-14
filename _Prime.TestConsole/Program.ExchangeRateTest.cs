using System;
using System.Threading;
using System.Threading.Tasks;
using Prime.Common;
using Prime.Common.Exchange.Rates;
using Prime.Utility;

namespace TestConsole
{
    public partial class Program
    {
        public class ExchangeRateTest
        {
            private object _lock = new object();

            public void Test()
            {
                var messenger = DefaultMessenger.I.Default;
                messenger.Register<NewLogMessage>(this, m =>
                {
                    Console.WriteLine(m.Message);
                });
                /*
                var exch = LatestPriceAggregator.I;
                exch.AddRequest(this, new AssetPair("cann", "usd"));
                //exch.AddRequest(new AssetPair("btc", "usd"));
                //exch.AddRequest(new AssetPair("usd", "btc"));

                void AddRequest3()
                {
                    exch.AddRequest(this, new AssetPair("cann", "eur"));
                }

                void RemoveRequest1()
                {
                    exch.RemoveRequest(this, new AssetPair("cann", "eur"));
                }

                void RemoveRequest2()
                {
                    exch.RemoveRequest(this, new AssetPair("usd", "btc"));
                }

                var once = false;
                void RunOnce()
                {
                    if (once)
                        return;

                    once = true;

                    new Task(() =>
                    {
                        Thread.Sleep(2000);
                        RemoveRequest2();
                    }).Start();

                    new Task(() =>
                    {
                        Thread.Sleep(8000);
                        RemoveRequest1();
                    }).Start();

                    new Task(() =>
                    {
                        Thread.Sleep(1000);
                        AddRequest3();
                    }).Start();
                }
                */
                /*messenger.Register<ExchangeRateCollected>(this, m =>
                {
                    Console.WriteLine(m.UtcCreated.ToLongTimeString() + ": " + m.Pair + " = " + m.Price.Asset + " " + m.Price.ToDecimalPointString(10));
                    RunOnce();
                });
                
                messenger.Register<LatestPricesUpdatedMessage>(this, re =>
                {
                    lock (_lock)
                    {
                        Console.WriteLine("--------- RESULTS ------------");
                        foreach (var m in exch.Results())
                            Console.WriteLine(m.UtcCreated.ToLongTimeString() + ": " + m.Pair + " = " + m.Price.Asset +
                                              " " + m.Price.ToDecimalPointString(10));

                        Console.WriteLine("--------- END ------------");
                        RunOnce();
                    }
                });

                do
                {
                    Thread.Sleep(1);
                } while (1 == 1);*/
            }
        }
    }
}