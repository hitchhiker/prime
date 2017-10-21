namespace Prime.Common
{
    public interface IOhlcResolutionAdapterStorage : IOhlcResolutionAdapter
    {
        void StoreRange(OhlcData data, TimeRange rangeAttempted);

        CoverageMapBase CoverageMap { get; }
    }
}