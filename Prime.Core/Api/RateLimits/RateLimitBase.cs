using System;

namespace Prime.Core
{
    public abstract class RateLimitBase : IRateLimiter
    {
        public DateTime UtcLastHit;

        public virtual void Hit()
        {
            UtcLastHit = DateTime.UtcNow;
        }

        public abstract bool IsSafe();
    }
}