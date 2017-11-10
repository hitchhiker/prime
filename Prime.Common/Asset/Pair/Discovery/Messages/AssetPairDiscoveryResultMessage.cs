using System.Collections.Generic;
using System.Linq;

namespace Prime.Common
{
    public class AssetPairDiscoveryResultMessage
    {
        public readonly AssetPairNetworks DiscoverFirst;
        public readonly IReadOnlyList<AssetPairNetworks> Discovered;
        public readonly AssetPairDiscoveryRequestMessage RequestRequestMessage;
        public readonly bool IsFailed;

        public AssetPairDiscoveryResultMessage(AssetPairDiscoveryRequestMessage requestRequest, AssetPairNetworks discoverFirst, IEnumerable<AssetPairNetworks> discovered)
        {
            Discovered = discovered.ToList();
            RequestRequestMessage = requestRequest;
            DiscoverFirst = discoverFirst;
            IsFailed = DiscoverFirst == null || DiscoverFirst.Providers.Count == 0;
        }

        public AssetPairDiscoveryResultMessage(AssetPairDiscoveryRequestMessage requestRequest)
        {
            RequestRequestMessage = requestRequest;
            IsFailed = true;
        }
    }
}