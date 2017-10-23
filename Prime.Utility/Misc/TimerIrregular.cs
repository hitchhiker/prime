using System;
using System.Threading.Tasks;
using System.Timers;

namespace Prime.Utility
{
    /// <inheritdoc />
    /// <summary>
    /// Fires immediately, then fires after each elapsed time plus the time it takes to execute the action.
    /// </summary>
    public class TimerIrregular : IDisposable
    {
        private readonly Action _action;
        private readonly Timer _timer;
        private readonly object _lock = new object();

        public TimerIrregular(TimeSpan frequencySpan, Action action)
        {
            _action = action;
            _timer = new Timer
            {
                Interval = frequencySpan.TotalMilliseconds,
                AutoReset = false
            };
            _timer.Elapsed += _timer_Elapsed;
        }

        public void Start()
        {
            lock (_lock)
                _timer_Elapsed(null, null);
        }

        public void Stop()
        {
            lock (_lock)
                _timer.Stop();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            new Task(delegate
            {
                lock (_lock)
                {
                    _action.Invoke();
                    _timer.Start();
                }
            }).Start();
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _timer?.Stop();
                _timer?.Dispose();
            }
        }
    }
}