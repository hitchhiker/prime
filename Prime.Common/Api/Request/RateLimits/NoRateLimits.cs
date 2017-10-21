namespace Prime.Common
{
    public class NoRateLimits : IRateLimiter
    {
        public void Limit() { }

        public bool IsSafe(NetworkProviderContext context)
        {
            return true;
        }
    }
}