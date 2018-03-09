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
        public IEnumerable<ProviderModel> GetNetworks()
        {
            return Networks.I.Providers.AsEnumerable().Select(x => new ProviderModel()
            {
                Name = x.Network.Name,
                Id = x.Network.Id.ToString()
            });
        }
        public ProviderDetailsModel GetProviderDetails(string objectId)
        {
            var network = Networks.I.Providers.AsEnumerable()
                .FirstOrDefault(x => x.Network.Id.Equals(objectId.ToObjectId()));

            ProviderDetailsModel result = null;

            if (network != null)
            {
                var apiKey = UserContext.Current.GetApiKey(network);

                result = new ProviderDetailsModel() { Name = network.Network.Name, Id = objectId};

                if (apiKey != null)
                {
                    result.Extra = apiKey.Extra;
                    result.Key = apiKey.Key;
                    result.Secret = apiKey.Secret;
                }
            }

            return result;
        }

        public void SaveKeys(string providerId, string key, string secret, string extra)
        {
            var provider = Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>()
                .FirstOrDefault(x => x.Network.Id.ToString().Equals(providerId));

            if (provider == null)
                throw new NullReferenceException("Provider not found.");

            var keys = UserContext.Current.ApiKeys;

            var keysToRemove = new List<ApiKey>();
            foreach (var apiKey in keys)
            {
                if (apiKey.Network.Id.Equals(provider.Network.Id))
                    keysToRemove.Add(apiKey);
            }

            foreach (var apiKey in keysToRemove)
            {
                keys.Remove(apiKey);
            }

            var newKey = new ApiKey(provider.Network, provider.Title, key, secret, string.IsNullOrEmpty(extra) ? null : extra);
            keys.Add(newKey);
            keys.Save();
        }

        public IEnumerable<ProviderModel> GetPrivateNetworks(bool direct = true)
        {
            var userContext = UserContext.Current;
            return Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>()
                .Where(x => x.IsDirect == direct).Select(x =>
                    new ProviderModel()
                    {
                        Name = x.Network.Name,
                        Id = x.Network.Id.ToString(),
                        HasKeys = userContext.GetApiKey(x) != null
                    });
        }
    }
}