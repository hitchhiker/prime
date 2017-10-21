using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common
{
    public interface ICoinInformationProvider : INetworkProvider, IDescribesAssets
    {
        Task<List<AssetInfo>> GetCoinInfoAsync(NetworkProviderContext context);
    }
}
