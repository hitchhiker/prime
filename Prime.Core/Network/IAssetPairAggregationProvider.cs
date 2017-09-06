namespace Prime.Core
{
    public interface IAssetPairAggregationProvider : INetworkProvider
    {
        AssetExchangeData GetCoinInfo(AggregatedCoinInfoContext context);

        void RefreshCoinInfo(AssetExchangeData assetData);
    }
}