namespace Prime.Core
{
    public interface IDescribesAssets : INetworkProvider
    {
        IAssetCodeConverter GetAssetCodeConverter();
    }
}