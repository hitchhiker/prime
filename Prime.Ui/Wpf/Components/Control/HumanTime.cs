using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Humanizer;

namespace prime
{
    public class HumanTime : TextBlock
    {
        public HumanTime()
        {
            _dispatcher = Application.Current.Dispatcher;
            SetValue();

            _timer = new Timer
            {
                AutoReset = true,
                Interval = Interval
            };

            _timer.Elapsed += delegate { SetValue(); };

            _timer.Enabled = true;
            this.Unloaded += HumanTime_Unloaded;
        }

        private void HumanTime_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Enabled = false;
            _timer.Dispose();
        }
       
        private readonly Timer _timer;
        private readonly Dispatcher _dispatcher;

        private void SetValue()
        {
            if (!_dispatcher.HasShutdownStarted)
                _dispatcher.Invoke(() =>
                {
                    Text = DateTime == DateTime.MinValue || DateTime == DateTime.MaxValue ? null : DateTime.Humanize();
                });
        }

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(
                nameof(DateTime),
                typeof(DateTime),
                typeof(HumanTime),
                new PropertyMetadata(default(DateTime), DateTimeChanged));

        private static void DateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as HumanTime;
            if (e.NewValue is DateTime && e.OldValue is DateTime)
            {
                var nt = (DateTime)e.NewValue;
                var ot = (DateTime)e.OldValue;
                if (nt != ot)
                {
                    tb.DateTime = (DateTime)e.NewValue;
                    tb.SetValue();
                    return;
                }
            }
            else
                if (tb.DateTime == DateTime.MinValue)
                    return;

            tb.DateTime = DateTime.MinValue;
            tb.SetValue();
        }

        /// <summary>
        /// Interval in milliseconds
        /// </summary>
        public int Interval { get; set; } = 200;

        public DateTime DateTime { get; set; }
    }
}