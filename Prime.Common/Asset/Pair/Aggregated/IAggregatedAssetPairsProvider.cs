using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAggregatedAssetPairsProvider : IDescribesAssets
    {
        Task<AssetPairByNetwork> GetAssetPairsByNetwork(NetworkProviderContext context);
    }
}