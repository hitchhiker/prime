using Prime.Core;

namespace plugins
{
    public class CryptoCompareRateLimiter : RateLimitBase
    {
        public override bool IsSafe()
        {
            return true;
        }
    }
}