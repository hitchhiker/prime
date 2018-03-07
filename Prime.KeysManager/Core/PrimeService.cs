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

                result = new ProviderDetailsModel() { Name = network.Network.Name };

                if (apiKey != null)
                {
                    result.Extra = apiKey.Extra;
                    result.Key = apiKey.Key;
                    result.Secret = apiKey.Secret;
                }
            }

            return result;
        }

        public IEnumerable<ProviderModel> GetPrivateNetworks(bool direct = true)
        {
            return Networks.I.Providers.AsEnumerable().FilterType<INetworkProvider, INetworkProviderPrivate>().Where(x => x.IsDirect == direct).Select(x => new ProviderModel()
            {
                Name = x.Network.Name,
                Id = x.Network.Id.ToString()
            });
        }
    }
}