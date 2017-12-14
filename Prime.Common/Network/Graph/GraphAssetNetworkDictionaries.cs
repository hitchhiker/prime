using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class GraphAssetNetworkDictionaries
    {
        private Dictionary<Network, IReadOnlyList<AssetPair>> _pairsByNetwork;
        
        private UniqueList<AssetPair> _assetPairs;
        private IReadOnlyList<Asset> _assets;
        private UniqueList<Network> _networks;

        public IReadOnlyList<AssetPair> AssetPairs => _assetPairs;
        public IReadOnlyDictionary<AssetPair, IReadOnlyList<Network>> NetworksByPair => NetworksByPairLazy;
        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> PairsByNetwork => _pairsByNetwork;
        public IReadOnlyList<Asset> Assets => _assets;
        public IReadOnlyList<Network> Networks => _networks;

        private readonly object _nbpl = new object();
        private Dictionary<AssetPair, IReadOnlyList<Network>> _networksByPairBacking;
        private Dictionary<AssetPair, IReadOnlyList<Network>> NetworksByPairLazy
        {
            get
            {
                lock (_nbpl)
                    return _networksByPairBacking ?? (_networksByPairBacking = FastInvert(_networks, _pairsByNetwork));
            }
        }

        public void BuildData(IReadOnlyCollection<KeyValuePair<Network, IReadOnlyList<AssetPair>>> data)
        {
            _pairsByNetwork = data.ToDictionary(x => x.Key, y => y.Value.AsReadOnlyList());
            _assetPairs = _pairsByNetwork.SelectMany(x => x.Value).ToUniqueList();
            _assets = _assetPairs.Select(a1 => a1.Asset1).Union(_assetPairs.Select(a2 => a2.Asset2)).ToUniqueList();
            _networks = _pairsByNetwork.Keys.ToUniqueList();
            _networksByPairBacking = null;
        }

        private static Dictionary<AssetPair, IReadOnlyList<Network>> FastInvert(UniqueList<Network> networks, Dictionary<Network, IReadOnlyList<AssetPair>> pbn)
        {
            var temp = new Dictionary<AssetPair, List<Network>>();
            foreach (var n in networks)
            {
                var pairs = pbn.Get(n);
                if (!pairs.Any())
                    continue;
                foreach (var p in pairs)
                {
                    if (!temp.ContainsKey(p))
                        temp.Add(p, new List<Network>());
                    temp[p].Add(n);
                }
            }
            return temp.ToDictionary(x => x.Key, y => y.Value.AsReadOnlyList());
        }

        public void BuildData(GraphAssetNetworkDictionaries d, bool normalise)
        {
            var source = normalise
                ? d.PairsByNetwork.ToDictionary(x => x.Key, y => y.Value.Select(x => x.Normalised).AsReadOnlyList())
                : d.PairsByNetwork;
            BuildData(source);
        }

        /// <summary>
        /// Creates a set intersection of both keys and values.
        /// </summary>
        public void BuildData(IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> p1, IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> p2)
        {
            var keys = p1.Keys.Intersect(p2.Keys);
            var d = new Dictionary<Network, IReadOnlyList<AssetPair>>();
            foreach (var k in keys)
            {
                var l1 = p1.Get(k);
                var l2 = p2.Get(k);
                if (l1 == null && l2 == null)
                    continue;

                if (l1 == null && l2 != null)
                    d.Add(k, l2.ToList());
                else if (l2 == null && l1 != null)
                    d.Add(k, l1.ToList());
                else
                    d.Add(k, l1.Intersect(l2).ToList());
            }

            BuildData(d);
        }

        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> DeNormalise(IReadOnlyCollection<KeyValuePair<Network, IReadOnlyList<AssetPair>>> rawData)
        {
            var d = new Dictionary<Network, IReadOnlyList<AssetPair>>();
            foreach (var kv in PairsByNetwork)
            {
                var rawPairs = rawData.Get(kv.Key);
                if (rawPairs == null)
                    continue;

                var vals = new UniqueList<AssetPair>();
                foreach (var ourPair in kv.Value)
                {
                    if (rawPairs.Contains(ourPair))
                        vals.Add(ourPair);
                    else if (rawPairs.Contains(ourPair.Reversed))
                        vals.Add(ourPair.Reversed);
                }

                d.Add(kv.Key, vals);
            }
            return d;
        }
    }
}