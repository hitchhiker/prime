using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAssetPairsProvider : INetworkProvider
    {
        Task<AssetPairs> GetAssetPairs(NetworkProviderContext context);
    }
}