using System.Threading.Tasks;

namespace Prime.Common
{
    public interface ICoinSnapshotAggregationProvider : INetworkProvider
    {
        Task<AggregatedAssetPairData> GetCoinSnapshotAsync(AssetPairDataContext context);
    }
}