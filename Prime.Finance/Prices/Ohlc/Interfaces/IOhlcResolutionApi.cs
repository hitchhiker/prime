using Prime.Core;

namespace Prime.Finance
{
    public interface IOhlcResolutionApi
    {
        OhlcResolutionAdapter Adapter { get; }

        OhlcData GetRange(TimeRange timeRange);
    }
}