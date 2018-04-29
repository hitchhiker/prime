using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Finance
{
    public interface ICoinSnapshotAggregationProvider : INetworkProvider
    {
        Task<AggregatedAssetPairData> GetCoinSnapshotAsync(AssetPairDataContext context);
    }
}