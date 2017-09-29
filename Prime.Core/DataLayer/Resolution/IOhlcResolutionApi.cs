namespace Prime.Core
{
    public interface IOhlcResolutionApi
    {
        OhlcResolutionAdapter Adapter { get; }

        OhlcData GetRange(TimeRange timeRange);
    }
}