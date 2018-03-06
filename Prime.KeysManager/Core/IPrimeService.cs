using System.Collections;
using System.Collections.Generic;
using Prime.Common;

namespace Prime.KeysManager.Core
{
    public interface IPrimeService
    {
        IEnumerable<Network> GetNetworks();
        IEnumerable<Network> GetPrivateNetworks(bool direct = true);
        
    }
}