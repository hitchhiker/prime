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

        private readonly Dictionary<AssetPairDiscoveryRequestMessage, AssetPairDiscoveries> _cache = new Dictionary<AssetPairDiscoveryRequestMessage, AssetPairDiscoveries>();

        private readonly object _lock = new object();

        private AssetPairDiscoveries GetOrCreateInstance(AssetPairDiscoveryRequestMessage request)
        {
            lock (_lock)
                return _cache.GetOrAdd(request, k => new AssetPairDiscoveries(k));
        }

        internal AssetPairDiscoveries Discover(AssetPairDiscoveryRequestMessage request)
        {
            var i = GetOrCreateInstance(request);
            i.Wait(TimeSpan.FromSeconds(120));
            return !i.IsFinished() ? null : i;
        }
    }
}