using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace Prime.Ui.Wpf.View
{
    /// <summary>
    /// Interaction logic for AfkMonitor.xaml
    /// </summary>
    public partial class AfkMonitor : UserControl
    {
        public AfkMonitor()
        {
            InitializeComponent();
            _timer.Tick += Timer_Tick;
        }
        
        private bool _readyToShowAfkMessage;
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public static readonly DependencyProperty idleLimitProperty = DependencyProperty.Register("IdleSecondsLimit", typeof(int), typeof(AfkMonitor), new FrameworkPropertyMetadata(int.MinValue));

        public int IdleSecondsLimit
        {
            get => (int)GetValue(idleLimitProperty);
            set => SetValue(idleLimitProperty, value);
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            //If process is running.
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length >= 1)
            {
                int idleTimeMilliseconds = 0;
                LastInputInfo lastInputInfo = new LastInputInfo();
                lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
                lastInputInfo.dwTime = 0;

                int tickCount = Environment.TickCount;

                if (GetLastInputInfo(out lastInputInfo))
                {
                    int lastInputTick = lastInputInfo.dwTime;
                    idleTimeMilliseconds = tickCount - lastInputTick;
                }

                int idleTimeSeconds;

                if (idleTimeMilliseconds > 0)
                    idleTimeSeconds = idleTimeMilliseconds / 1000;
                else
                    idleTimeSeconds = idleTimeMilliseconds;

                if (idleTimeSeconds > IdleSecondsLimit && _readyToShowAfkMessage)
                {
                    _readyToShowAfkMessage = false;
                    MessageBox.Show("You are inactive!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (idleTimeSeconds <= IdleSecondsLimit)
                {
                    _readyToShowAfkMessage = true;
                }
            }
        }

        private void AfkMonitor_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void AfkMonitor_OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(out LastInputInfo plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LastInputInfo
        {
            public static readonly int SizeOf =
                Marshal.SizeOf(typeof(LastInputInfo));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int dwTime;
        }
    }
}
