namespace Prime.Common
{
    public interface IOhlcResolutionApi
    {
        OhlcResolutionAdapter Adapter { get; }

        OhlcData GetRange(TimeRange timeRange);
    }
}