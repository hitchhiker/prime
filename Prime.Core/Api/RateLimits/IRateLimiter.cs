namespace Prime.Core
{
    public interface IRateLimiter
    {
        void Limit();

        bool IsSafe(NetworkProviderContext context);
    }
}