using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IAssetPairsProvider : IDescribesAssets
    {
        Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context);
    }
}