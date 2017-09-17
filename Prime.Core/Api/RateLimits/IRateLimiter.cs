namespace Prime.Core
{
    public interface IRateLimiter
    {
        void Hit();

        bool IsSafe();
    }
}