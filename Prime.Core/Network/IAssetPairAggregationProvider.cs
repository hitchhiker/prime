using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IAssetPairAggregationProvider : INetworkProvider
    {
        Task<AssetExchangeData> GetCoinInfoAsync(AggregatedCoinInfoContext context);

        Task<bool> RefreshCoinInfoAsync(AssetExchangeData assetData);
    }
}