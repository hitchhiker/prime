namespace Prime.Core
{
    public interface IOhclResolutionApi
    {
        OhlcResolutionAdapter Adapter { get; }

        OhclData GetRange(TimeRange timeRange);
    }
}