using System.Collections;
using System.Collections.Generic;
using Prime.Core;
using Prime.KeysManager.Core.Models;

namespace Prime.KeysManager.Core
{
    public interface IApiKeyService
    {
        IEnumerable<NetworkModel> GetNetworks();
        IEnumerable<NetworkModel> GetPrivateNetworks(bool direct = true);

        NetworkDetailsModel GetNetworkDetails(string objectId);
        void SaveKeys(string networkId, string key, string secret, string extra);
        void DeleteKeys(string networkId);

        bool TestPrivateApi(string networkId, string key, string secret, string extra);
    }
}