using Prime.Common;

namespace Prime.Common
{
    public interface IAggregator : INetworkProvider
    {
        UniqueList<Network> NetworksSupported { get; }
    }
}