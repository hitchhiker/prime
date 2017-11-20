using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Nito.AsyncEx;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class NetworkGraph
    {
        private readonly GraphAssetNetworkDictionaries _dR = new GraphAssetNetworkDictionaries();
        private readonly GraphAssetNetworkDictionaries _dN = new GraphAssetNetworkDictionaries();

        public readonly NetworkContext Context;
        public readonly GraphMeta Meta;
        public readonly PriceGraph PriceGraph;
        
        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> PairsByNetworkRaw => _dR.PairsByNetwork;
        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> PairsByNetwork => _dN.PairsByNetwork;
        public IReadOnlyDictionary<AssetPair, IReadOnlyList<Network>> NetworksByPair => _dN.NetworksByPair;
        public IReadOnlyList<Asset> Assets => _dN.Assets;
        public IReadOnlyList<AssetPair> AssetPairs => _dN.AssetPairs;
        public IReadOnlyList<Network> Networks => _dN.Networks;
        
        public NetworkGraph(NetworkContext context, PriceGraph priceGraph)
        {
            Context = context;
            PriceGraph = priceGraph;
            Meta = new GraphMeta(context);

            var fromPrices = priceGraph.PairsByNetwork.ToDictionary(x => x.Key, y => y.Value);

            _dR.BuildData(fromPrices);
            _dN.BuildData(_dR, true);
        }

        public NetworkGraph(NetworkContext context)
        {
            Context = context;
            Meta = new GraphMeta(context);

            var orig = AsyncContext.Run(() => AssetPairProvider.I.GetNetworksDiskAsync());

            var pbn = orig.ToDictionary(x => x.Key, y => y.Value.Select(x => x.Normalised).AsReadOnlyList());
            var pbnR = orig.ToDictionary(x => x.Key, y => y.Value as IReadOnlyList<AssetPair>);

             ApplyFilters(Context, pbn);

            _dN.BuildData(pbn);
            _dR.BuildData(pbnR);
        }

        private static void ApplyFilters(NetworkContext ctx, Dictionary<Network, IReadOnlyList<AssetPair>> pbn)
        {
            if (ctx.OnlyNetworks?.Any() != true)
                pbn.RemoveAll(x => ctx.BadNetworks.Contains(x.Key));
            else
                pbn.RemoveAll(x => !ctx.OnlyNetworks.Contains(x.Key));

            if (ctx.OnlyPairs?.Any() != true)
            {
                var clone = pbn.ToDictionary(x => x.Key, y => y.Value);
                foreach (var kv in clone)
                    pbn[kv.Key] = new AssetPairs(kv.Value.Where(p => !ctx.BadCoins.Contains(p.Asset1) && !ctx.BadCoins.Contains(p.Asset2)));
            }
            else
                ApplyOnlyPairs(ctx, pbn);

            RemoveFaulting(pbn);
        }

        private static void ApplyOnlyPairs(NetworkContext ctx, Dictionary<Network, IReadOnlyList<AssetPair>> pbn)
        {
            if (ctx.OnlyPairs != null || !ctx.OnlyPairs.Any())
                return;

            var opn = ctx.OnlyPairs.Select(x => x.Normalised).ToList();

            var clone = pbn.ToDictionary(x => x.Key, y => y.Value);
            foreach (var kv in clone)
                pbn[kv.Key] = new AssetPairs(pbn[kv.Key].Intersect(opn));

            pbn.RemoveAll(x => !x.Value.Any());
        }

        private static void RemoveFaulting(Dictionary<Network, IReadOnlyList<AssetPair>> pbn)
        {
            var hasFault = false;

            var networks = pbn.Keys.ToList();
            foreach (var n in networks)
            {
                var prov = n.Providers.FirstOf<IDepositProvider>();
                if (prov == null)
                    continue;

                var r = ApiCoordinator.GetTransferSuspensions(prov);
                if (r.IsNull)
                    continue;

                var pairs = pbn[n].AsList();
                var c = pairs.Count;
                pairs.RemoveAll(x => r.Response.DepositSuspended.Any(x.Has));
                pairs.RemoveAll(x => r.Response.WithdrawalSuspended.Any(x.Has));
                hasFault = hasFault || c !=pairs.Count;

                pbn[n] = pairs;
            }

            if (hasFault)
                pbn.RemoveAll(x => !x.Value.Any());
        }

        public AssetPair GetOriginalPair(Network network, AssetPair pair)
        {
            if (_dR.NetworksByPair.Get(pair)?.Contains(network) == true)
                return pair;
            if (_dR.NetworksByPair.Get(pair.Reversed)?.Contains(network) == true)
                return pair.Reversed;
            return null;
        }
    }
}
