using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Prime.Common;
using Prime.KeysManager.Core.Models;
using Prime.Utility;

namespace Prime.KeysManager.Core
{
    public class PrimeService : IPrimeService
    {
        private Network GetNetworkById(string networkId)
        {
            var provider = Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>()
                .FirstOrDefault(x => x.Network.Id.ToString().Equals(networkId));

            return provider?.Network;
        }

        private INetworkProvider GetProviderByNetworkId(string networkId)
        {
            var provider = Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>()
                .FirstOrDefault(x => x.Network.Id.ToString().Equals(networkId));

            return provider;
        }

        public IEnumerable<NetworkModel> GetNetworks()
        {
            return Networks.I.Providers.AsEnumerable().Select(x => new NetworkModel()
            {
                Name = x.Network.Name,
                Id = x.Network.Id.ToString()
            });
        }

        public NetworkDetailsModel GetNetworkDetails(string objectId)
        {
            var networkProvider = Networks.I.Providers.AsEnumerable()
                .FirstOrDefault(x => x.Network.Id.Equals(objectId.ToObjectId()));

            NetworkDetailsModel result = null;

            if (networkProvider != null)
            {
                var apiKey = UserContext.Current.GetApiKey(networkProvider);

                result = new NetworkDetailsModel() { Name = networkProvider.Network.Name, Id = objectId};

                if (apiKey != null)
                {
                    result.Extra = apiKey.Extra;
                    result.Key = apiKey.Key;
                    result.Secret = apiKey.Secret;
                }
            }

            return result;
        }

        public void SaveKeys(string networkId, string key, string secret, string extra)
        {
            var networkProvider = Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>()
                .FirstOrDefault(x => x.Network.Id.ToString().Equals(networkId));

            if (networkProvider == null)
                throw new NullReferenceException("Network provider not found.");

            DeleteKeys(networkId);

            var keys = UserContext.Current.ApiKeys;

            if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
                throw new NullReferenceException("Key and secret should not be empty.");

            var newKey = new ApiKey(networkProvider.Network, networkProvider.Title, key, secret, string.IsNullOrEmpty(extra) ? null : extra);
            keys.Add(newKey);

            keys.Save();
        }

        public void DeleteKeys(string networkId)
        {
            var keys = UserContext.Current.ApiKeys;

            var keysToRemove = new List<ApiKey>();
            foreach (var apiKey in keys)
            {
                if (apiKey.Network.Id.ToString().Equals(networkId))
                    keysToRemove.Add(apiKey);
            }

            foreach (var apiKey in keysToRemove)
            {
                keys.Remove(apiKey);
            }

            keys.Save();
        }

        public bool TestPrivateApi(string networkId)
        {
            var network = GetProviderByNetworkId(networkId) as INetworkProviderPrivate;

            var result = network.TestPrivateApiAsync(new ApiPrivateTestContext(UserContext.Current.GetApiKey(network))).Result;

            return result;
        }

        public IEnumerable<NetworkModel> GetPrivateNetworks(bool direct = true)
        {
            var userContext = UserContext.Current;
            return Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>()
                .Where(x => x.IsDirect == direct).Select(x =>
                    new NetworkModel()
                    {
                        Name = x.Network.Name,
                        Id = x.Network.Id.ToString(),
                        HasKeys = userContext.GetApiKey(x) != null
                    });
        }
    }
}