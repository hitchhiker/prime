using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Prime.Base;
using Prime.Core;

namespace Prime.Core
{
    public sealed class Networks : IEnumerable<Network>
    {
        public static Networks I => Lazy.Value;
        private static readonly Lazy<Networks> Lazy = new Lazy<Networks>(()=>new Networks());
        private readonly object _lock = new object();
        public readonly UniqueList<Network> KnownNetworks = new UniqueList<Network>();

        private readonly ConcurrentDictionary<ObjectId, Network> _cache = new ConcurrentDictionary<ObjectId, Network>();

        public Network Get(string name)
        {
            lock (_lock)
            {
                if (!_collected)
                {
                    _collected = true;
                    KnownNetworks.AddRange(TypeCatalogue.I.ImplementInstances<INetworkProvider>().Where(x=>!x.Disabled).Select(x => x.Network));
                }
            }
            return _cache.GetOrAdd(Network.GetHash(name), k => new Network(name));
        }

        private bool _collected;
        private IReadOnlyList<INetworkProvider> _providers;
        public IReadOnlyList<INetworkProvider> Providers => _providers ?? (_providers = TypeCatalogue.I.ImplementInstances<INetworkProvider>().Where(x=>!x.Disabled).OrderByDescending(x=>x.Priority).ToList());

        private readonly ConcurrentDictionary<Type, IReadOnlyList<INetworkProvider>> _providersTypeDictionary = new ConcurrentDictionary<Type, IReadOnlyList<INetworkProvider>>();

        public IReadOnlyList<T> GetProviders<T>()
        {
            return _providersTypeDictionary.GetOrAdd(typeof(T), x => Providers.OfList<T>().OfType<INetworkProvider>().AsReadOnlyList()) as IReadOnlyList<T>;
        }

        public T GetProvider<T>() where T : INetworkProvider
        {
            return GetProviders<T>().FirstProvider();
        }

        public IEnumerator<Network> GetEnumerator()
        {
            return _cache.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_cache.Values).GetEnumerator();
        }
    }
}