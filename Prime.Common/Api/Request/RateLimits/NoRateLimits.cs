namespace Prime.Common
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