using System;
using System.Collections.Generic;
using System.Threading;

namespace Prime.Common
{
    public class PerHourRateLimiter : IRateLimiter
    {
        private readonly int _anonyRequests;
        private readonly int _anonyPerHour;
        private readonly int _requests;
        private readonly int _perHour;

        private readonly List<DateTime> _hits = new List<DateTime>();
        private readonly object _lock = new object();

        public PerHourRateLimiter(int requests, int perHour)
        {
            _anonyRequests = requests;
            _anonyPerHour = perHour;
            _requests = requests;
            _perHour = perHour;
        }

        public PerHourRateLimiter(int anonyRequests, int anonyPerHour, int requests, int perHour)
        {
            _anonyRequests = anonyRequests;
            _anonyPerHour = anonyPerHour;
            _requests = requests;
            _perHour = perHour;
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

                var expired = DateTime.UtcNow.AddHours(-_perHour);
                _hits.RemoveAll(x => x <= expired);
                return _hits.Count < _requests;
            }
        }
    }
}