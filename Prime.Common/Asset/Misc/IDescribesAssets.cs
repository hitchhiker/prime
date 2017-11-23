namespace Prime.Common
{
    public interface IDescribesAssets : INetworkProvider
    {
        IAssetCodeConverter GetAssetCodeConverter();

        char? CommonPairSeparator { get; }
    }
}