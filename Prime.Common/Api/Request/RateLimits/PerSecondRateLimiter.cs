using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Prime.Common
{
    public class PerSecondRateLimiter : IRateLimiter
    {
        private readonly int _anonyRequests;
        private readonly int _anonyPerSeconds;
        private readonly int _requests;
        private readonly int _perSeconds;

        private readonly List<DateTime> _hits = new List<DateTime>();
        private readonly object _lock = new object();

        public PerSecondRateLimiter(int requests, int perSeconds)
        {
            _anonyRequests = requests;
            _anonyPerSeconds = perSeconds;
            _requests = requests;
            _perSeconds = perSeconds;
        }

        public PerSecondRateLimiter(int anonyRequests, int anonyPerSeconds, int requests, int perSeconds)
        {
            _anonyRequests = anonyRequests;
            _anonyPerSeconds = anonyPerSeconds;
            _requests = requests;
            _perSeconds = perSeconds;
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

                var expired = DateTime.UtcNow.AddSeconds(-_perSeconds);
                _hits.RemoveAll(x => x <= expired);
                return _hits.Count < _requests;
            }
        }
    }
}
