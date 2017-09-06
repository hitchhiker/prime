namespace Prime.Core
{
    public interface IOhlcResolutionAdapterStorage : IOhlcResolutionAdapter
    {
        void StoreRange(OhclData data, TimeRange rangeAttempted);

        CoverageMapBase CoverageMap { get; }
    }
}