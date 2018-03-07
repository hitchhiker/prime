using System.Collections;
using System.Collections.Generic;
using Prime.Common;
using Prime.KeysManager.Core.Models;

namespace Prime.KeysManager.Core
{
    public interface IPrimeService
    {
        IEnumerable<ProviderModel> GetNetworks();
        IEnumerable<ProviderModel> GetPrivateNetworks(bool direct = true);

        ProviderDetailsModel GetProviderDetails(string objectId);
    }
}