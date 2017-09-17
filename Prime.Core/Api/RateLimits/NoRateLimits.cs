namespace Prime.Core
{
    public class NoRateLimits : RateLimitBase
    {
        public override bool IsSafe()
        {
            return true;
        }
    }
}