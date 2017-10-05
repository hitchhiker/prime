using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IAssetPairAggregationProvider : INetworkProvider
    {
        Task<AssetPairData> GetCoinSnapshotAsync(AssetPairDataContext context);
    }
}