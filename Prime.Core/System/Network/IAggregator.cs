using Prime.Core;

namespace Prime.Core
{
    public interface IAggregator : INetworkProvider
    {
        UniqueList<Network> NetworksSupported { get; }
    }
}