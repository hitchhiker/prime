using Prime.Core;

namespace Prime.Finance
{
    public interface IDescribesAssets : INetworkProvider
    {
        IAssetCodeConverter GetAssetCodeConverter();

        char? CommonPairSeparator { get; }
    }
}