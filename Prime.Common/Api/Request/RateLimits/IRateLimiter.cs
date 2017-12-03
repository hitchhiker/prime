namespace Prime.Common
{
    public interface IRateLimiter
    {
        void Limit();

        bool IsSafe();
    }
}