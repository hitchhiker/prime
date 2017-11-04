using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Nito.AsyncEx;
using plugins;
using Prime.Common;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class FrankTests
        {
            private IMessenger _m = DefaultMessenger.I.Default;

            public FrankTests()
            {
                var done = false;
                var apd = new AssetPairDiscoveryRequestMessage(new AssetPair("XRP", "USD"));
                _m.Register<AssetPairDiscoveryResultMessage>(this, m =>
                {
                    Console.WriteLine(string.Join(", ", m.Networks.Networks.Select(x => x.Name)));
                    done = true;
                });

                _m.Send(apd);

                do
                {
                    Thread.Sleep(1);
                } while (!done);

                /*
                var p1 = Networks.I.Providers.FirstProviderOf<CoinfloorCryptoCompareProvider>() as IAssetPairsProvider;
                var r1 = ApiCoordinator.GetAssetPairs(p1);

                foreach (var i in r1.Response)
                    Console.WriteLine(i);

                var p = Networks.I.Providers.FirstProviderOf<CryptoCompareProvider>();
                var r = AsyncContext.Run(() => p.GetAssetPairsAllNetworksAsync());

                foreach (var i in r)
                    Console.WriteLine(i.Key.Name + " " + i.Value.Count);

                var pub = PublicContext.I.PubData;
                var d = new Dictionary<Network, AssetPairs>();
                foreach (var prov in Networks.I.AssetPairsProviders)
                {
                    var r = ApiCoordinator.GetAssetPairs(prov);
                    if (r.IsFailed)
                    {
                        Console.WriteLine(prov.Title + " could not perform " + nameof(ApiCoordinator.GetAssetPairs));
                        return;
                    }

                    d.Add(prov.Network, r.Response);
                }

                var pairs = d.SelectMany(x => x.Value).ToUniqueList();

                foreach (var pair in pairs)
                {
                    var apd = pub.GetAggAssetPairData(pair);
                    if (apd.IsMissing)
                    {
                        Console.WriteLine($"No AGG data for {pair}");
                        continue;
                    }

                    var aggNets = apd.Exchanges.Select(x => x.Network).ToUniqueList();
                    var apNets = d.Where(x => x.Value.Contains(pair)).Select(x => x.Key).ToUniqueList();

                    foreach (var n in apNets)
                    {
                        if (aggNets.Contains(n))
                            continue;
                        Console.WriteLine($"{pair} not found via network: {n.Name}");
                    }
                }*/
            }
        }
    }
}