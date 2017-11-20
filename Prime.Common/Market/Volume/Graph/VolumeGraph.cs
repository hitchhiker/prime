using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Core.Market;
using Prime.Utility;

namespace Prime.Common
{
    public class VolumeGraph
    {
        private readonly GraphAssetNetworkDictionaries _dN = new GraphAssetNetworkDictionaries();

        public readonly VolumeContext Context;
        public readonly NetworkGraph NetworkGraph;
        public readonly PriceGraph PriceGraph;
        public readonly VolumeDbProvider VolumeDbProvider;
        public readonly GraphMeta Meta;
        private readonly IReadOnlyList<NetworkPairVolume> _volume;
        private readonly Dictionary<Network, IList<AssetPair>> _missingPairsByNetwork = new Dictionary<Network, IList<AssetPair>>();
        private readonly Dictionary<Network, IList<AssetPair>> _lowVolumePairsByNetwork = new Dictionary<Network, IList<AssetPair>>();
        private readonly UniqueList<AssetPair> _failedBtc = new UniqueList<AssetPair>();

        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> PairsByNetwork => _dN.PairsByNetwork;
        public IReadOnlyDictionary<AssetPair, IReadOnlyList<Network>> NetworksByPair => _dN.NetworksByPair;
        public IReadOnlyList<Asset> Assets => _dN.Assets;
        public IReadOnlyList<AssetPair> AssetPairs => _dN.AssetPairs;
        public IReadOnlyList<Network> Networks => _dN.Networks;
        public IReadOnlyList<NetworkPairVolume> Volume => _volume;

        public IReadOnlyDictionary<AssetPair, IReadOnlyList<NetworkPairVolume>> VolumeByPair => VolumeDbProvider.Data.ByPair;
        public IReadOnlyDictionary<Network, IReadOnlyList<NetworkPairVolume>> VolumeByNetwork => VolumeDbProvider.Data.ByNetworks;
        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> MissingPairsByNetwork => _missingPairsByNetwork.ToDictionary(x=>x.Key, y=>y.Value.AsReadOnlyList());
        public IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> LowVolumePairsByNetwork => _lowVolumePairsByNetwork.ToDictionary(x => x.Key, y => y.Value.AsReadOnlyList());
        public IReadOnlyList<AssetPair> FailedBtc => _failedBtc;

        public VolumeGraph(VolumeContext context, NetworkGraph networkGraph, PriceGraph priceGraph)
        {
            Context = context;
            NetworkGraph = networkGraph;
            PriceGraph = priceGraph;
            Meta = networkGraph.Meta;

            _dN.BuildData(PriceGraph.PairsByNetwork);

            var id = "VolumeGraph".GetObjectIdHashCode();

            if (context.FlushVolume)
                VolumeDbProvider.Clear(id);

            VolumeDbProvider = new VolumeDbProvider(id);

            _volume = GetAllVolumeData();

            ApplyBtc();

            FilterVolume(_dN.PairsByNetwork);

            VolumeExceptionCheck();
        }

        private IReadOnlyList<NetworkPairVolume> GetAllVolumeData()
        {
            var alldata = VolumeDbProvider.GetAllVolume(_dN.PairsByNetwork, (p, n) =>
            {
                if (Context.FlushVolume)
                    Console.Write("Getting Volume: " + p.Name + " " + n + ": ");
            }, (p, n, d) =>
            {
                if (!Context.FlushVolume)
                    return;

                if (d == null)
                    Console.Write("Missing" + Environment.NewLine);
                else
                    Console.Write(d.Volume24 + Environment.NewLine);
            });
            
            return alldata.Volume;
        }

        private void ApplyBtc()
        {
            foreach (var i in _volume)
                if (!i.ApplyBtcVolume(PriceGraph.Prices) && !i.HasVolume24BaseBtc && !i.HasVolume24QuoteBtc)
                    _failedBtc.Add(i.Pair);
        }

        private void FilterVolume(IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> pbn)
        {
            var r = new Dictionary<Network, IReadOnlyList<AssetPair>>();
            foreach (var kv in pbn)
            {
                var pairs = new UniqueList<AssetPair>();
                foreach (var pair in kv.Value)
                {
                    if (!VolumeCheck(kv.Key, pair))
                        continue;
                    
                    pairs.Add(pair);
                }

                if (pairs.Any())
                    r.Add(kv.Key, pairs);
            }

            _dN.BuildData(r);
        }

        private bool VolumeCheck(Network network, AssetPair pair)
        {
            var pairVolume = VolumeDbProvider.GetVolume(network, pair);
            if (pairVolume == null || (!pairVolume.HasVolume24Quote && !pairVolume.HasVolume24Base))
            {
                if (AssumeVolume(pair))
                    return true;

                var mps = _missingPairsByNetwork.GetOrAdd(network, n => new UniqueList<AssetPair>());
                mps.Add(pair);
                return false;
            }

            var vol1 = pairVolume.Volume24QuoteBtc ?? pairVolume.Volume24BaseBtc;
            var vol2 = pairVolume.Volume24BaseBtc ?? pairVolume.Volume24QuoteBtc;

            vol1 = vol2 = vol1 ?? vol2;
            var minBtc = vol1 < vol2 ? vol1 : vol2;

            var hasvolume = minBtc > 40;
            if (hasvolume)
                return true;

            var mps2 = _lowVolumePairsByNetwork.GetOrAdd(network, n => new UniqueList<AssetPair>());
            mps2.Add(pair);

            return false;
        }

        private bool AssumeVolume(AssetPair pair)
        {
            if (!Context.AllowVolumeAssumptions)
                return false;

            return Meta.AssumeHighVolume.Any(x => x.EqualsOrReversed(pair)) || Meta.HighVolumeAssets.Any(pair.Has);
        }

        private void VolumeExceptionCheck()
        {
            foreach (var kv in _dN.PairsByNetwork)
            {
                var c = _volume.Count(x => x.Network.Id == kv.Key.Id && kv.Value.Contains(x.Pair)) + kv.Value.Count(AssumeVolume);
                if (c < kv.Value.Count)
                    throw new Exception("Missing Volume: " + kv.Key.Name);
            }
        }
    }
}