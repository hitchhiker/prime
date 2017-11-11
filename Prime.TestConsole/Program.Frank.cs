using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Nito.AsyncEx;
using plugins;
using Prime.Common;
using Prime.Common.Prices.Latest;
using Prime.Core;
using Prime.Utility;

namespace Prime.TestConsole
{
    public partial class Program
    {
        public class FrankTests
        {
            private IMessenger _m = DefaultMessenger.I.Default;

            private IDictionary<Network, AssetPairs> _networkGraph; 
            private IDictionary<IPublicPriceSuper, AssetPairs> _provGraph; 
            private IDictionary<Network, MarketPricesData> _priceGraph; 

            public FrankTests()
            {
                var starta = "BTC".ToAssetRaw();
                var startn = Networks.I.Get("bitstamp");

                _networkGraph = AsyncContext.Run(() => AssetPairProvider.I.GetNetworksDiskAsync()) as IDictionary<Network, AssetPairs>;

                BuildProviderGraph();
                BuildPriceGraph();

                Console.WriteLine("Graphs loaded.");

                var d = new Dictionary<IPublicPriceSuper, MarketPrice>();

                DoAssetPairs(startn.Providers.OfType<IPublicPriceSuper>().FirstDirectProvider(), starta);

                /*
                foreach (var n in pn)
                {
                    var r = ApiCoordinator.GetPrice(n, new PublicPriceContext(start));
                    if (r.IsNull)
                        continue;
                    d.Add(n, r.Response);
                }

                var std = new Money((decimal) d.Values.Select(x => x.Price.ToDoubleValue()).StandardDeviation(), start.Asset2);

                Console.WriteLine("ST Deviation: " + std);*/

                foreach (var i in d.OrderBy(x => x.Value.Price))
                   Console.WriteLine(i.Key.Title + " " + i.Value.Price);

                Thread.Sleep(20000);
            }

            private void BuildProviderGraph()
            {
                _provGraph = new Dictionary<IPublicPriceSuper, AssetPairs>();
                foreach (var kv in _networkGraph)
                {
                    var prov = kv.Key.PublicPriceProviders.FirstDirectProvider<IPublicPriceSuper>();
                    if (prov == null)
                    {
                        Console.WriteLine($"No provider found for {kv.Key.Name}");
                        continue;
                    }
                    _provGraph.Add(prov, kv.Value);
                }
            }

            private void BuildPriceGraph()
            {
                _priceGraph = new Dictionary<Network, MarketPricesData>();
                foreach (var kv in _networkGraph)
                {
                    var e = PublicContext.I.As<MarketPricesData>().FirstOrDefault(x => x.Id == MarketPricesData.GetHash(kv.Key)) ?? GrabPriceData(kv);
                    if (e != null)
                        _priceGraph.Add(kv.Key, e);
                }
            }

            private static MarketPricesData GrabPriceData(KeyValuePair<Network, AssetPairs> kv)
            {
                Console.WriteLine($"Getting price data for {kv.Key.Name} from {kv.Value.Count} pairs.");

                var prov = kv.Key.PublicPriceProviders.FirstDirectProvider<IPublicPriceSuper>();
                if (prov == null)
                {
                    Console.WriteLine($"No provider found for {kv.Key.Name}");
                    return null;
                }
                var r = ApiCoordinator.GetPrices(prov, new PublicPricesContext(kv.Value.ToList()));
                if (r.IsNull)
                {
                    Console.WriteLine($"No prices found for {kv.Key.Name}");
                    return null;
                }

                var data = new MarketPricesData(kv.Key, r.Response.MarketPrices);
                data.SavePublic();
                return data;
            }

            public void DoAssetPairs(IPublicPriceSuper prov, Asset asset)
            {
                var pairs = _provGraph.Get(prov).Where(x => x.Has(asset));
                var prices = ApiCoordinator.GetPrices(prov, new PublicPricesContext(pairs.ToList())).Response;
                foreach (var i in prices.MarketPrices)
                    Console.WriteLine(i);
            }

            public void Old() { 
            /*
            var msg = new AssetPairAllRequestMessage().WaitForResponse<AssetPairAllRequestMessage, AssetPairAllResponseMessage>();

            var pairs = msg.Pairs;
            foreach (var p in pairs)
            {
                Console.WriteLine(p);
            }



            /*
            Console.WriteLine("CANN:USD");

            var nets = AssetPairDiscovery.I.Discover(new AssetPairDiscoveryRequestMessage(new AssetPair("CANN", "USD")));
            foreach (var i in nets.Discovered.OrderByDescending(x=>x.Sort).ThenByDescending(x=>x.TotalNetworksInvolved))
                Console.WriteLine(i.TotalNetworksInvolved + " " + i.SortB + " "+  i.Sort + " " + string.Join(", ", i.Networks.Select(x => x.Name)));

            Console.WriteLine("USDT:USD");

            nets = AssetPairDiscovery.I.Discover(new AssetPairDiscoveryRequestMessage(new AssetPair("USDT", "USD")));
            foreach (var i in nets.Discovered.OrderByDescending(x => x.Sort).ThenByDescending(x => x.TotalNetworksInvolved))
                Console.WriteLine(i.TotalNetworksInvolved + " " + i.SortB + " " + i.Sort + " " + string.Join(", ", i.Networks.Select(x => x.Name)));

           
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