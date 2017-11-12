using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
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

            private readonly IReadOnlyList<Asset> _fiat = "USD,EUR,JPY".ToCsv().Select(x => x.ToAssetRaw()).ToList();

            private readonly IDictionary<Network, AssetPairs> _assetGraph; 
            private IDictionary<Network, MarketPricesData> _priceGraph;
            private List<AssetPairNetworkRoutes> _routes;
            private IDictionary<Network, ExchangeHops> _knownHops;
            private readonly bool _flushPrices = false;
            private readonly AssetPair _testAssetPair = null;

            public FrankTests()
            {
                //_flushPrices = true;
                //_testAssetPair = "BTC_EMC".ToAssetPairRaw();

                var starta = "BTC".ToAssetRaw();
                var startn = Networks.I.Get("bitstamp");

                _assetGraph = AsyncContext.Run(() => AssetPairProvider.I.GetNetworksDiskAsync()) as IDictionary<Network, AssetPairs>;

                if (_testAssetPair!=null)
                    BuildTestData();

                BuildAndNormalisePriceGraph();

                NormaliseAssetGraph();

                Console.WriteLine("Graphs loaded.");

                BuildRoutes();

                BuildKnownHops();

               // WriteConsoleSummary();

                var paths = FindPaths(null, startn, starta).Take(10).ToList();

                foreach (var path in paths.OrderByDescending(x=>x.FirstOrDefault().GetCompoundPercent()))
                {
                    var route = path.FirstOrDefault();
                    Console.WriteLine($"{Environment.NewLine}{path.Id} +{Math.Round(route.GetCompoundPercent() - 100, 2)}% {route.Explain()}");
                }
            }

            private UniqueList<ObjectId> _yielded = new UniqueList<ObjectId>();

            private IEnumerable<RoutePath> FindPaths(RoutePath path, Network network, Asset assetTransfer)
            {
                var routes = _knownHops.Get(network);
                if (routes == null)
                    yield break;

                var possibleHops = routes.OrderByDescending(x=>x.Percentage).Where(x => x.ForAsset(assetTransfer)).Where(x => Equals(x.NetworkLow, network)).Where(x=> !_fiat.Contains(x.AssetTransfer));
                if (!possibleHops.Any())
                    yield break;
                
                foreach (var hop in possibleHops)
                {
                    var newPath = path == null ? new RoutePath(network) : path.Clone();
                    var stage = new RouteStage(hop, assetTransfer);

                    if (newPath.Has(stage) || hop.NetworkHigh.Equals(newPath.StartNetwork) || newPath.Count > 30)
                        continue;
                    
                    newPath.Add(stage);
                    
                    foreach (var r in FindPaths(newPath, hop.NetworkHigh, stage.AssetFinal))
                        yield return r;
                }

                if (_yielded.Add(path.Id))
                    yield return path;
            }

            private void WriteConsoleSummary()
            {
                Console.WriteLine($"--------------{_routes.Count} ROUTES ------------");
                Console.WriteLine();
                Console.WriteLine("---------------ALL USD---------------");
                Console.WriteLine();

                foreach (var summary in _routes.Where(x => x.Pair.Has(Asset.Usd)).OrderByDescending(x => x.BestPercentage))
                    Console.WriteLine(summary);

                Console.WriteLine();
                Console.WriteLine("---------------ABOVE 4%--------------");

                Console.WriteLine();
                foreach (var summary in _routes.Where(x => x.BestPercentage >= 4).OrderByDescending(x => x.BestPercentage))
                    Console.WriteLine(summary);
            }

            private void BuildTestData()
            {
                restart:
                foreach (var kv in _assetGraph)
                {
                    var e = _assetGraph[kv.Key].FirstOrDefault(x => x.EqualsOrReversed(_testAssetPair));
                    if (e==null)
                    {
                        _assetGraph.Remove(kv.Key);
                        goto restart;
                    }
                    _assetGraph[kv.Key] = new AssetPairs(new List<AssetPair>() {e});
                }
            }

            private void BuildRoutes()
            {
                var allpairs = _priceGraph.SelectMany(x => x.Value.Prices.Select(p=>p.Pair)).ToUniqueList();
                _routes = new List<AssetPairNetworkRoutes>();
                foreach (var pair in allpairs)
                {
                    if (!pair.IsNormalised)
                        throw new Exception("Not normalised");

                    var prices = GetPrices(pair);
                    if (prices.Count < 2)
                        continue;

                    prices = prices.OrderBy(x => x.Price).ToList();
                    _routes.Add(new AssetPairNetworkRoutes(pair, prices));
                }
            }

            private void BuildKnownHops()
            {
                _knownHops = new Dictionary<Network, ExchangeHops>();
                foreach (var i in _routes.SelectMany(x => x))
                {
                    var rts =  _knownHops.GetOrAdd(i.NetworkLow, k => new ExchangeHops());
                    rts.Add(i);
                }
            }

            public void DoAssetPairs(Network network, Asset asset)
            {
                var prices = _priceGraph.Get(network);
                foreach (var i in prices.Prices)
                    Console.WriteLine(i);
            }

            private List<MarketPrice> GetPrices(AssetPair pair)
            {
                var r = new List<MarketPrice>();
                var nets = _assetGraph.Where(x => x.Value.Contains(pair)).Select(x => x.Key).ToUniqueList();
                foreach (var n in nets)
                {
                    var p = _priceGraph.Get(n);
                    var apiM = p?.Prices.FirstOrDefault(x => x.Pair.Equals(pair));

                    if (apiM == null)
                        continue;

                    r.Add(apiM);
                }
                return r;
            }

            private void NormaliseAssetGraph()
            {
                foreach (var kv in _priceGraph)
                {
                    var ne = _assetGraph.Get(kv.Key);
                    if (ne == null)
                        throw new Exception(nameof(_assetGraph) + " missing " + kv.Key.Name);
                    
                    _assetGraph[kv.Key] = new AssetPairs(kv.Value.Prices.Select(x => x.Pair));
                }
            }

            private void BuildAndNormalisePriceGraph()
            {
                _priceGraph = new Dictionary<Network, MarketPricesData>();
                foreach (var kv in _assetGraph)
                {
                    var db = kv.Key.NameLowered!="###" ? PublicContext.I.As<MarketPricesData>().FirstOrDefault(x => x.Id == MarketPricesData.GetHash(kv.Key)) : null;

                    if (_flushPrices)
                        db = null;

                    var e = db ?? GrabPriceData(kv);
                    if (e == null)
                        continue;

                    var prices = e.Prices.ToList();
                    prices.RemoveAll(x => x.Price == 0);

                    if (_testAssetPair != null)
                        prices.RemoveAll(x => !x.Pair.EqualsOrReversed(_testAssetPair));

                    var todo = prices.Where(x => !x.Pair.IsNormalised).ToList();
                    prices.RemoveAll(x => todo.Contains(x));

                    foreach (var i in todo)
                        prices.Add(i.Reverse());

                    _priceGraph.Add(kv.Key, new MarketPricesData(e.Network, prices));

                    Console.WriteLine($"Retrieved price data for {kv.Key.Name}: {prices.Count} / {kv.Value.Count} pairs.");
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