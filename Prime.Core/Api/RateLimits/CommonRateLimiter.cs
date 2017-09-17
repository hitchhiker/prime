using System;

namespace Prime.Core
{
    public class CommonRateLimiter : RateLimitBase
    {
        public CommonRateLimiter(int requests, int perMinutes)
        {
            
        }

        public override bool IsSafe()
        {
            return false;
        }
    }
}