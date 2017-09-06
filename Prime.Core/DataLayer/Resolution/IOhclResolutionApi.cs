namespace Prime.Core
{
    public interface IOhclResolutionApi
    {
        OhlcResolutionDataAdapter Adapter { get; }

        OhclData GetRange(TimeRange timeRange);
    }
}