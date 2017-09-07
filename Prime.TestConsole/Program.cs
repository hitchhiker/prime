using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Ipfs.Api;
using Prime.Core;
using plugins;
using Prime.Core.Wallet;
using Prime.Radiant.Components;
using Prime.Utility;
using Prime.Radiant;
using Prime.Radiant.Components.IPFS.Messenging;

namespace TestConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger(Console.WriteLine);
            var radiant = new Radiant(logger);
            Logging.I.OnNewMessage += I_OnNewMessage;

            IpfsName(radiant);

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

        private static void IpfsName(Radiant radiant)
        {
            var n = new FileSystemNode {Hash = "ABCDE"};

            var usr = UserContext.Current.IpfsMessenger;

            usr.UserMessages.AddOrUpdate("wallet", n);

            Console.WriteLine(usr.Publish().Result);

            Console.WriteLine(usr.Retrieve("QmVyJYeEugVn9HrKnTufWwPiWSq7v234YYSvYeENvFj8iM").Result);
        }

        private static void I_OnNewMessage(object sender, EventArgs e)
        {
            var ev = e as LoggerMessageEvent;
            Console.WriteLine(ev.Level + ": " + ev.Message);
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

        private static void BalanceTest()
        {
            var provider = Networks.I.WalletProviders.OfType<PoloniexProvider>().FirstProvider();
            var c = new PortfolioProviderScannerContext(UserContext.Current, provider, UserContext.Current.BaseAsset, 0);
            var scanner = new PortfolioProviderScanner(c);
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

            var ohcl = new OhlcDataAdapter(new OhlcResolutionAdapterAdapterContext() {Pair = pair});

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
