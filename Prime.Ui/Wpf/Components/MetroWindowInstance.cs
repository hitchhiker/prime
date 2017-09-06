using System.Windows;
using Prime.Core;
using MahApps.Metro.Controls;
using prime;

namespace Prime.Ui.Wpf
{
    public class MetroWindowInstance : WindowInstanceBase
    {
        public MetroWindowInstance() { }

        public MetroWindow MetroWindow => _win;

        private MetroWindow _win;

        public MetroWindowInstance(MetroWindow metroWindow)
        {
            _win = metroWindow;
            Sync();
        }

        public void Restore()
        {
            if (Width != 0 && Height != 0)
            {
                _win.Top = Top;
                _win.Left = Left;
                _win.Width = Width;
                _win.Height = Height;
            }

            switch (OpenState)
            {
                case WindowOpenState.Fullscreen:
                case WindowOpenState.Maximised:
                    _win.WindowState = WindowState.Normal;
                    _win.Show();
                    _win.WindowState = WindowState.Maximized;
                    break;
                case WindowOpenState.Normal:
                    _win.WindowState = WindowState.Normal;
                    _win.Show();
                    break;
                case WindowOpenState.None:
                    if (PreviousOpenState == WindowOpenState.Maximised)
                    {
                        _win.WindowState = WindowState.Normal;
                        _win.WindowState = WindowState.Maximized;
                        _win.Show();
                        _win.WindowState = WindowState.Minimized;
                        break;
                    }
                    _win.WindowState = WindowState.Normal;
                    _win.Show();
                    _win.WindowState = WindowState.Minimized;
                    break;
            }

        }

        public void AttachWindow(MetroWindow window)
        {
            _win = window;
        }

        public void Sync()
        {
            switch (_win.WindowState)
            {
                case WindowState.Maximized:
                    SetState(WindowOpenState.Maximised);
                    break;
                case WindowState.Normal:
                    SetState(WindowOpenState.Normal);
                    break;
                case WindowState.Minimized:
                    SetState(WindowOpenState.None);
                    break;
            }

            Top = _win.Top;
            Left = _win.Left;
            Width = _win.Width;
            Height = _win.Height;
        }

        public void ToTop()
        {
            _win.Topmost = true;
            _win.Topmost = false;
            _win.Focus();
        }

        public void Minimise()
        {
            _win.WindowState = WindowState.Minimized;
            Sync();
        }

        public void Show()
        {
            if (_win.WindowState != WindowState.Minimized)
                return;

            if (PreviousOpenState == WindowOpenState.None)
                _win.WindowState = WindowState.Normal;
            else
                _win.WindowState = PreviousOpenState == WindowOpenState.Normal ? WindowState.Normal : WindowState.Maximized;
            Sync();
        }

        public void Close()
        {
            _win.Close();
        }

        public static MetroWindowInstance Design => new MetroWindowInstance(new MetroWindow());
    }
}