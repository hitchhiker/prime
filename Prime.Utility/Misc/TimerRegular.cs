using System;
using System.Threading.Tasks;
using System.Timers;

namespace Prime.Utility
{
    /// <inheritdoc />
    /// <summary>
    /// Fires immediately, then fires after each elapsed time on a TASK thread.
    /// </summary>
    public class TimerRegular : IDisposable
    {
        private readonly Action _action;
        private readonly Timer _timer;

        public TimerRegular(TimeSpan frequencySpan, Action action, bool startNow = false)
        {
            _action = action;
            _timer = new Timer
            {
                Interval = frequencySpan.TotalMilliseconds,
                AutoReset = true
            };
            _timer.Elapsed += _timer_Elapsed;

            if (startNow)
                Start();

            _timer.Start();
        }

        public void Start()
        {
            _timer_Elapsed(null, null);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            new Task(delegate
            {
                _action.Invoke();
            }).Start();
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}