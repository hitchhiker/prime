using Prime.Utility;

namespace Prime.Common
{
    public interface IAggregator : INetworkProvider
    {
        UniqueList<Network> NetworksSupported { get; }
    }
}