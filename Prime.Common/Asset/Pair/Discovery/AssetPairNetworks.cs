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
        public readonly bool IsPairReversed;

        public AssetPairNetworks(AssetPair pair, IReadOnlyList<Network> networks, bool isPairReversed = false)
        {
            Pair = pair;
            Networks = networks;
            IsPairReversed = isPairReversed;
        }

        private List<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers => _providers ?? (_providers = Networks.SelectMany(x => x.Providers).Distinct().ToList());

        public AssetPair OriginalPair => IsPairReversed ? Pair.Reverse() : Pair;

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

        public AssetPairNetworks ConversionPart1 { get; set; }

        public AssetPairNetworks ConversionPart2 { get; set; }

        public bool IsConverting => ConversionPart1 != null || ConversionPart2 != null;
    }
}