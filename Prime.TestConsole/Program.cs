using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ipfs.Api;
using LiteDB;
using Nito.AsyncEx;
using Prime.Core;
using plugins;
using Prime.Core.Wallet;
using Prime.Plugins.Services.BitMex;
using Prime.Plugins.Services.Kraken;
using Prime.Radiant.Components;
using Prime.Utility;
using Prime.Radiant;
using Prime.Radiant.Components.IPFS.Messenging;
using Prime.TestConsole;
using AssetPair = Prime.Core.AssetPair;

namespace TestConsole
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            // Kraken.

            // new Prime.TestConsole.Program.KrakenTests().GetBalances();
            // new Prime.TestConsole.Program.KrakenTests().GetAssetPairs();
            // new Prime.TestConsole.Program.KrakenTests().GetLatestPrice();
            // new Prime.TestConsole.Program.KrakenTests().GetFundingMethod();
            // new Prime.TestConsole.Program.KrakenTests().GetOhlc();
            // new Prime.TestConsole.Program.KrakenTests().GetDepositAddresses();

            // BitMex.

            // new Prime.TestConsole.Program.BitMexTests().GetOhlcData();

            // BitStamp.

            // new Prime.TestConsole.Program.BitStampTests().GetTicker();

            // Poloniex.

            // new Prime.TestConsole.Program.PoloniexTests().GetBalances();
            // new Prime.TestConsole.Program.PoloniexTests().ApiTest();
            new Prime.TestConsole.Program.PoloniexTests().AssetsTest();


            //Sha256Test();
            //new ExchangeRateTest().Test();

            //LatestPricesTest();
            //LatestPriceTest();

            //Worker worker = new Worker();
            //worker.Run();

            //var logger = new Logger(Console.WriteLine);
            //var radiant = new Radiant(logger);
            //Logging.I.OnNewMessage += I_OnNewMessage;

            //IpfsName(radiant);

            /*Task.Run(async () =>
            {
                Console.WriteLine(await Go());
            }).GetAwaiter().GetResult();
            */

            //DeployTest();

            //BalanceTest();

            //DataTest();
            //OhclTest();
        }

        private static void Sha256Test()
        {
            var auth = new KrakenAuthenticator(null);
            var sha = auth.HashSHA256("test");
        }



        private static void IpfsName(Radiant radiant)
        {
            var n = new FileSystemNode { Hash = "ABCDE" };

            var usr = UserContext.Current.IpfsMessenger;

            usr.UserMessages.AddOrUpdate("wallet", n);

            Console.WriteLine(usr.Publish().Result);

            Console.WriteLine(usr.Retrieve("QmVyJYeEugVn9HrKnTufWwPiWSq7v234YYSvYeENvFj8iM").Result);
        }

        /*
        private static async Task<string> Go()
        {
           var client = new IpfsClient();
            var v = await client.VersionAsync();
            var fsn = await client.FileSystem.AddDirectoryAsync(@"C:\Users\hitchhiker\AppData\Local\Prime\publish\test", true);
            return fsn.Hash;
        }*/

        private static void DeployTest()
        {
            var pc = PublishManagerContext.LoadDefault(null);
            var pub = new PublishManager(pc);

            pub.Start();
        }

        private static void LatestPricesTest()
        {
            var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();

            var ctx = new PublicPricesContext("BTC".ToAssetRaw(), new List<Asset>()
            {
                "ETH".ToAsset(provider),
                "ETC".ToAsset(provider),
                "DASH".ToAsset(provider),
                "LTC".ToAsset(provider),
                "QTUM".ToAsset(provider),
                "XMR".ToAsset(provider),
                "XRP".ToAsset(provider),
                "XTZ".ToAsset(provider),
                "ZEC".ToAsset(provider),
            });

            try
            {
                var c = AsyncContext.Run(() => provider.GetLatestPricesAsync(ctx));

                Console.WriteLine($"Base asset: {ctx.BaseAsset}\n");

                foreach (Money price in c.Prices)
                {
                    Console.WriteLine(price.Display);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static void LatestPriceTest()
        {
            var provider = Networks.I.Providers.OfType<BitMexProvider>().FirstProvider();
            var ctx = new PublicPriceContext(new AssetPair("XBT".ToAsset(provider), "USD".ToAsset(provider)));

            try
            {
                var c = AsyncContext.Run(() => provider.GetLatestPriceAsync(ctx));
                Console.WriteLine(c.Price.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void BalanceTest()
        {
            var provider = Networks.I.OfType<BitMexProvider>().FirstProvider();
            //provider.GetAssetPairs


            var c = new PortfolioProviderContext(UserContext.Current, provider, UserContext.Current.BaseAsset, 0);
            var scanner = new PortfolioProvider(c);
            try
            {
                scanner.Update();
                foreach (var i in scanner.Items)
                    Console.WriteLine(i.Asset.ShortCode + " " + i.AvailableBalance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        private static void DataTest()
        {
            var a1 = "BTC".ToAssetRaw();
            var a2 = "USD".ToAssetRaw();
            var pair = new AssetPair(a1, a2);

            var ohcl = new OhlcDataAdapter(new OhlcResolutionContext() { Pair = pair });

            ohcl.Init();

            Console.WriteLine(ohcl.UtcDataStart.ToLongDateString());
        }

        private static void OhclTest()
        {
            /*
            var a1 = "BTC".ToAssetRaw();
            var a2 = "USD".ToAssetRaw();
            var pair = new AssetPair(a1, a2);

            var ohcl = new OhlcResolutionDataAdapter(pair, TimeResolution.Day) {StorageEnabled = false};

            var range = new TimeRange(TimeSpan.FromDays(-365*10), TimeResolution.Day);

            var r = ohcl.Request(range);
            if (r == null)
                Console.WriteLine("NULL");
            else
                foreach (var i in r)
                    Console.WriteLine(i.Open + " " + i.Close);*/
        }
    }
}
