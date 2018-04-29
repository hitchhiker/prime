using System.Threading.Tasks;

namespace Prime.Core
{
    public interface ICoinSnapshotAggregationProvider : INetworkProvider
    {
        Task<AggregatedAssetPairData> GetCoinSnapshotAsync(AssetPairDataContext context);
    }
}