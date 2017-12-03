using System;
using System.Collections.Generic;
using System.Threading;
using Prime.Utility;

namespace Prime.Common
{
    public class PerMinuteRateLimiter : IRateLimiter
    {
        private readonly int _anonyRequests;
        private readonly int _anonyPerMinutes;
        private readonly int _requests;
        private readonly int _perMinutes;

        private readonly List<DateTime> _hits = new List<DateTime>();
        private readonly object _lock = new object();

        public PerMinuteRateLimiter(int requests, int perMinutes)
        {
            _anonyRequests = requests;
            _anonyPerMinutes = perMinutes;
            _requests = requests;
            _perMinutes = perMinutes;
        }

        public PerMinuteRateLimiter(int anonyRequests, int anonyPerMinutes, int requests, int perMinutes)
        {
            _anonyRequests = anonyRequests;
            _anonyPerMinutes = anonyPerMinutes;
            _requests = requests;
            _perMinutes = perMinutes;
        }

        public void Limit()
        {
            do
            {
                Thread.Sleep(100);
            } while (!IsSafe(false));
        }

        public bool IsSafe()
        {
            return IsSafe(true);
        }

        private bool IsSafe(bool hit)
        {
            lock (_lock)
            {
                if (hit)
                    _hits.Add(DateTime.UtcNow);

                var expired = DateTime.UtcNow.AddMinutes(-_perMinutes);
                _hits.RemoveAll(x => x <= expired);
                return _hits.Count < _requests;
            }
        }
    }
}