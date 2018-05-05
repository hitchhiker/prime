namespace Prime.Core
{
    public class NoRateLimits : IRateLimiter
    {
        public void Limit() { }

        public bool IsSafe()
        {
            return true;
        }
    }
}