namespace Prime.Common
{
    public interface IDescribesAssets : INetworkProvider
    {
        IAssetCodeConverter GetAssetCodeConverter();

        string CommonPairSeparator { get; }
    }
}