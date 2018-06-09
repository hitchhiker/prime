using System.Collections.Generic;
using Prime.Base.Messaging.Manager.Models;

namespace Prime.Manager.Core
{
    public interface IApiKeyService
    {
        IEnumerable<NetworkModel> GetNetworks();
        IEnumerable<NetworkModel> GetPrivateNetworks(bool direct = true);

        NetworkDetailsModel GetNetworkDetails(string objectId);
        void SaveKeys(string networkId, string key, string secret, string extra);
        void DeleteKeys(string networkId);

        bool TestPrivateApi(string networkId, string key, string secret, string extra);
        IEnumerable<MarketModel> GetMarkets();
    }
}