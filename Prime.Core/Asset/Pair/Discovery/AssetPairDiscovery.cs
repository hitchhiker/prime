using System;
using System.Collections.Generic;
using Prime.Common;
using Prime.Utility;

namespace Prime.Core
{
    internal sealed class AssetPairDiscovery
    {
        private AssetPairDiscovery() {}

        public static AssetPairDiscovery I => Lazy.Value;
        private static readonly Lazy<AssetPairDiscovery> Lazy = new Lazy<AssetPairDiscovery>(()=>new AssetPairDiscovery());

        private readonly Dictionary<AssetPairDiscoveryRequestMessage, AssetPairDiscoveryInstance> _cache = new Dictionary<AssetPairDiscoveryRequestMessage, AssetPairDiscoveryInstance>();

        private readonly object _lock = new object();

        private AssetPairDiscoveryInstance GetOrCreateInstance(AssetPairDiscoveryRequestMessage request)
        {
            lock (_lock)
                return _cache.GetOrAdd(request, k => new AssetPairDiscoveryInstance(k));
        }

        internal AssetPairNetworks Discover(AssetPairDiscoveryRequestMessage request)
        {
            var i = GetOrCreateInstance(request);
            i.Wait(TimeSpan.FromSeconds(20));
            return !i.IsFinished() ? null : i.Networks;
        }
    }
}