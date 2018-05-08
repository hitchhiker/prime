using Prime.Core;

namespace Prime.Finance
{
    public interface IOhlcResolutionAdapterStorage : IOhlcResolutionAdapter
    {
        void StoreRange(OhlcData data, TimeRange rangeAttempted);

        CoverageMapBase CoverageMap { get; }
    }
}