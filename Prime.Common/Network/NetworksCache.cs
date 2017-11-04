using System;
using System.Collections.Concurrent;
using LiteDB;

namespace Prime.Common
{
    public class NetworksCache
    {
        private NetworksCache() {}

        public static NetworksCache I => Lazy.Value;
        private static readonly Lazy<NetworksCache> Lazy = new Lazy<NetworksCache>(()=>new NetworksCache());

        private readonly ConcurrentDictionary<ObjectId, Network> _cache = new ConcurrentDictionary<ObjectId, Network>();

        public Network Get(string name)
        {
            return _cache.GetOrAdd(Network.GetHash(name), k => new Network(name));
        }
    }
}