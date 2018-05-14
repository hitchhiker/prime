using System.Collections.Concurrent;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.Finance
{
    internal class AssetPairDiscoveryMessenger : IStartupMessenger
    {
        public IMessenger M { get; private set; }

        public void Start(ServerContext context)
        {
            M = context.M;
            M.RegisterAsync<AssetPairDiscoveryRequestMessage>(this, AssetPairProviderDiscoveryMessage);
        }

        public void Stop()
        {
            M?.UnregisterAsync(this);
        }

        private void AssetPairProviderDiscoveryMessage(AssetPairDiscoveryRequestMessage m)
        {
            var networks = AssetPairDiscovery.I.Discover(m);
            M.SendAsync(new AssetPairDiscoveryResultMessage(m, networks.DiscoverFirst, networks.Discovered));
        }
    }
}