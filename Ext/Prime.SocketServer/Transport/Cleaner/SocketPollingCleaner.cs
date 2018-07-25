using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Prime.SocketServer.Transport.Cleaner
{
    /// <summary>
    /// Checks if clients connected to the server are still alive by storing the time of last successful socket read/write operations and periodical polling.
    /// </summary>
    public class SocketPollingCleaner<TClientIdentifier>
    {
        private readonly object _lock = new object();

        private readonly ConcurrentDictionary<TClientIdentifier, DateTime> _clientActivityTime = new ConcurrentDictionary<TClientIdentifier, DateTime>();
        private readonly Timer _pollingTimer = new Timer();
        private readonly Func<TClientIdentifier, bool> _clientAliveFunc;

        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);

        public event EventHandler<PollingCleanerEventArgs<TClientIdentifier>> OnClientPollingFailed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientAliveFinc">Method that checks if client is available. Should not take long time.</param>
        public SocketPollingCleaner(Func<TClientIdentifier, bool> clientAliveFinc)
        {
            _clientAliveFunc = clientAliveFinc;
        }

        public void StartPolling()
        {
            _pollingTimer.Stop();
            _pollingTimer.Interval = PollingInterval.TotalMilliseconds;
            _pollingTimer.Elapsed += PollingTimerOnElapsed;
            _pollingTimer.Start();
        }

        private void PollingTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine($"Elapsed. Total clients: {_clientActivityTime.Count}.");
            // TODO: AY: optimize by not polling clients which recently have communicated.
            // TODO: AY: consider implementing synchronization between connected clients lists.
            // TODO: AY: with increasing number of clients Parallel.ForEach might be ineffective, consider using less number of threads for clients polling.
            Parallel.ForEach(_clientActivityTime, pair =>
            {
                var isAlive = _clientAliveFunc(pair.Key);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: '{pair.Key}' is {(isAlive ? "alive": "dead")}.");
                if (isAlive)
                    UpdateActivity(pair.Key);
                else
                {
                    lock (_lock)
                    {
                        OnClientPollingFailed?.Invoke(this,
                            new PollingCleanerEventArgs<TClientIdentifier>() { ClientId = pair.Key });
                    }
                }
            });
        }

        /// <summary>
        /// Updates activity time of specified client. If client does not exist adds to the collection of connected clients.
        /// </summary>
        /// <param name="clientId"></param>
        public void UpdateActivity(TClientIdentifier clientId)
        {
            lock (_lock)
            {
                if (_clientActivityTime.TryGetValue(clientId, out var lastUpdateTime))
                {
                    if (!_clientActivityTime.TryUpdate(clientId, DateTime.UtcNow, lastUpdateTime))
                        throw new InvalidOperationException($"Unable to update activity time of client '{clientId}'.");
                }
                else
                {
                    lastUpdateTime = DateTime.UtcNow;
                    if (!_clientActivityTime.TryAdd(clientId, lastUpdateTime))
                        throw new InvalidOperationException($"Unable to add client '{clientId}'.");
                }
            }
        }

        public void DeleteClient(TClientIdentifier clientId)
        {
            lock (_lock)
            {
                if (!_clientActivityTime.TryRemove(clientId, out var removedActivityTime))
                    throw new InvalidOperationException($"Unable to delete client '{clientId}'.");
            }
        }

        public DateTime GetLastUpdateTime(TClientIdentifier clientId)
        {
            if (!_clientActivityTime.TryGetValue(clientId, out var lastUpdateTime))
                throw new InvalidOperationException($"Unable to get last activity time of client '{clientId}'.");

            return lastUpdateTime;
        }
    }
}
