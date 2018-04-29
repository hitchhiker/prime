using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance
{
    public interface ICoinInformationProvider : INetworkProvider, IDescribesAssets
    {
        Task<List<AssetInfo>> GetCoinInformationAsync(NetworkProviderContext context);
    }
}
