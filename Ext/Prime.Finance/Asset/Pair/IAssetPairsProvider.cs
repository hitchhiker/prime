using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance
{
    public interface IAssetPairsProvider : IDescribesAssets
    {
        Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context);
    }
}