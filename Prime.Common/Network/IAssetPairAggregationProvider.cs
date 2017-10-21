using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IAssetPairAggregationProvider : INetworkProvider
    {
        Task<AggregatedAssetPairData> GetCoinSnapshotAsync(AssetPairDataContext context);
    }
}