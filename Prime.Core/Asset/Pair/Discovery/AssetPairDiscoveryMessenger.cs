using System.Collections.Concurrent;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core
{
    internal class AssetPairDiscoveryMessenger : IStartupMessenger
    {
        private readonly IMessenger _m = DefaultMessenger.I.Default;
        private readonly Dictionary<AssetPairDiscoveryRequestMessage, AssetPairDiscoveryProvider> _cache = new Dictionary<AssetPairDiscoveryRequestMessage, AssetPairDiscoveryProvider>();
        private readonly object _lock = new object();

        internal AssetPairDiscoveryMessenger()
        {
            _m.RegisterAsync<AssetPairDiscoveryRequestMessage>(this, AssetPairProviderDiscoveryMessage);
        }

        private void AssetPairProviderDiscoveryMessage(AssetPairDiscoveryRequestMessage m)
        {
            lock (_lock)
            {
                var provider = _cache.Get(m);
                if (provider != null)
                {
                    provider.SendMessage();
                    return;
                }
                _cache.Add(m, new AssetPairDiscoveryProvider(m));
            }
        }
    }
}