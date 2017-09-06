using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public interface ICoinListProvider : INetworkProvider, IDescribesAssets
    {
        List<AssetInfo> GetCoinList();
    }
}
