using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Prime.Common.Api.Request.RateLimits
{
    public class PerDayRateLimiter : IRateLimiter
    {
        private readonly int _anonyRequests;
        private readonly int _anonyPerDays;
        private readonly int _requests;
        private readonly int _perDays;

        private readonly List<DateTime> _hits = new List<DateTime>();
        private readonly object _lock = new object();

        public PerDayRateLimiter(int requests, int perDays)
        {
            _anonyRequests = requests;
            _anonyPerDays = perDays;
            _requests = requests;
            _perDays = perDays;
        }

        public PerDayRateLimiter(int anonyRequests, int anonyPerDays, int requests, int perDays)
        {
            _anonyRequests = anonyRequests;
            _anonyPerDays = anonyPerDays;
            _requests = requests;
            _perDays = perDays;
        }

        public void Limit()
        {
            Thread.Sleep(1000);
        }

        public bool IsSafe(NetworkProviderContext context)
        {
            lock (_lock)
            {
                _hits.Add(DateTime.UtcNow);
                var expired = DateTime.UtcNow.AddDays(-_perDays);
                _hits.RemoveAll(x => x <= expired);
                return _hits.Count < _requests;
            }
        }
    }
}
