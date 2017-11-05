using System;
using System.Collections.Generic;
using LiteDB;
using Prime.Utility;
using System.Linq;
using System.Threading.Tasks;

namespace Prime.Common
{
    public class AssetPairNetworks
    {
        public readonly AssetPair Pair;
        public readonly IReadOnlyList<Network> Networks;

        public AssetPairNetworks(AssetPair pair, IReadOnlyList<Network> networks)
        {
            Pair = pair;
            Networks = networks;
        }

        private List<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers => _providers ?? (_providers = Networks.SelectMany(x => x.Providers).Distinct().ToList());

        public bool Has<T>(bool onlyDirect = true) where T : INetworkProvider
        {
            return onlyDirect ? Providers.OfType<T>().Any(x => x.IsDirect) : Providers.OfType<T>().Any();
        }

        public T Provider<T>(bool onlyDirect = true) where T : INetworkProvider
        {
            var prov = Providers.OfList<T>().Where(x=>x.IsDirect).FirstProviderByVolume(Pair);
            if (prov== null && !onlyDirect)
                return Providers.OfList<T>().FirstProviderByVolume(Pair);
            return prov;
        }

        public Network Network<T>(bool onlyDirect = true) where T : INetworkProvider
        {
            return Provider<T>(onlyDirect)?.Network;
        }

        private readonly object _confirmedLock = new object();
        private readonly List<Tuple<IPublicPriceSuper, AssetPair>> _confirmed = new List<Tuple<IPublicPriceSuper, AssetPair>>();

        public Network NetworkConfirmedPricing(AssetPair pair)
        {
            var provs = Providers.OfList<IPublicPriceSuper>().OrderByVolume(pair).Where(x => x.IsDirect);
            var ctx = new PublicPriceContext(pair);
            Network net = null;

            foreach (var i in provs)
            {
                lock (_confirmedLock)
                    if (_confirmed.Any(x => x.Item1 == i && Equals(x.Item2, pair)))
                    {
                        net = i.Network;
                        break;
                    }

                if (ApiCoordinator.GetPrice(i, ctx).IsNull)
                    continue;

                net = i.Network;

                lock (_confirmedLock)
                    _confirmed.Add(new Tuple<IPublicPriceSuper, AssetPair>(i, pair));
                break;
            }

            return net;
        }

        public bool IsPegged { get; set; }

        public bool IsIntermediary { get; set; }

        public AssetPairNetworks ConversionPart1 => IsConversionPart1 ? this : ConversionOther;

        public AssetPairNetworks ConversionPart2 => IsConversionPart2 ? this : ConversionOther;

        public AssetPairNetworks ConversionOther { get; set; }

        public bool IsConversionPart1 { get; set; }

        public bool IsConversionPart2 { get; set; }

        public bool IsConverting => IsConversionPart1 || IsConversionPart2;

        public int TotalNetworksInvolved => Networks.Count + ConversionOther?.Networks.Count ?? Networks.Count;

        public int SortB => Math.Abs(Networks.Count - ConversionOther?.Networks.Count ?? 0);

        public int Sort => TotalNetworksInvolved - SortB + (Networks.Count>1 && (ConversionOther == null || ConversionOther.Networks.Count>1) ? 1000 : 0);
    }
}