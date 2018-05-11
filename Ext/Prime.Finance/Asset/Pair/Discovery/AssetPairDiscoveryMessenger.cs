using System.Collections.Concurrent;
using GalaSoft.MvvmLight.Messaging;
using Prime.Core;

namespace Prime.Finance
{
    internal class AssetPairDiscoveryMessenger : IStartupMessenger
    {
        private readonly IMessenger _m = DefaultMessenger.I.DefaultServer;
        
        internal AssetPairDiscoveryMessenger()
        {
            _m.RegisterAsync<AssetPairDiscoveryRequestMessage>(this, AssetPairProviderDiscoveryMessage);
        }

        private void AssetPairProviderDiscoveryMessage(AssetPairDiscoveryRequestMessage m)
        {
            var networks = AssetPairDiscovery.I.Discover(m);
            _m.SendAsync(new AssetPairDiscoveryResultMessage(m, networks.DiscoverFirst, networks.Discovered));
        }
    }
}