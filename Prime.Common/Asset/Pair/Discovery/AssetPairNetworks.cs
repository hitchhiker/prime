using System.Collections.Generic;
using LiteDB;
using Prime.Utility;
using System.Linq;

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

        public bool Has<T>() where T : INetworkProvider
        {
            return Providers.OfType<T>().Any();
        }

        public T Provider<T>() where T : INetworkProvider
        {
            return Providers.OfList<T>().FirstProviderByVolume(Pair);
        }

        public Network Network<T>() where T : INetworkProvider
        {
            return Provider<T>()?.Network;
        }

        public bool IsPegged { get; set; }

        public bool IsIntermediary { get; set; }

        public AssetPairNetworks ConversionPart1 { get; set; }

        public AssetPairNetworks ConversionPart2 { get; set; }

        public bool IsConverting => ConversionPart1 != null || ConversionPart2 != null;
    }
}