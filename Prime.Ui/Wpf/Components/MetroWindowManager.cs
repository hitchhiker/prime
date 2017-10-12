using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Prime.Core;
using MahApps.Metro.Controls;
using prime;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    public class MetroWindowManager : IWindowManager
    {
        private readonly UserContext _context;

        public MetroWindowManager(UserContext context) { _context = context; }

        private readonly List<MetroWindowInstance> _instances = new List<MetroWindowInstance>();

        private readonly Debouncer _debouncer = new Debouncer();

        public void CreateNewWindow()
        {
            var psh = SystemParameters.PrimaryScreenHeight;
            var psw = SystemParameters.PrimaryScreenWidth;

            var w = new Screen() {Height = psh * .9, Width = psw * .9, WindowStartupLocation = WindowStartupLocation.CenterScreen};
            var i = new MetroWindowInstance(w);
            _instances.Add(i);
            w.Show();
            RegisterEvents(w);
            i.Sync();
            Update();
        }

        public void Init()
        {
            var savedInstances = WindowInstances.I.Get<MetroWindowInstance>(_context);
            if (savedInstances.Count == 0)
            {
                CreateNewWindow();
                return;
            }

            foreach (var i in savedInstances)
            {
                _instances.Add(i);
                var w = new Screen();
                i.AttachWindow(w);
                i.Restore();
                RegisterEvents(w);
            }
        }

        private void RegisterEvents(MetroWindow metro)
        {
            metro.Closing += Window_Closing;
            metro.SizeChanged += (o, args) => Update();
            metro.StateChanged += (o, args) => Update();
            metro.LocationChanged += (o, args) => Update();
        }

        public void Toggle()
        {
            if (_instances.Any(x => x.IsMinimised))
            {
                ShowAll();
                AllToTop();
            } else if (CanMinimiseAll())
                MinimiseAll();
            else if (_instances.Count == 0)
                CreateNewWindow();
            else
            {
                ShowAll();
                AllToTop();
            }
        }

        private void AllToTop()
        {
            foreach (var i in _instances)
            {
                i.ToTop();
            }
        }

        public void Update()
        {
            _debouncer.Debounce(500, o => UpdateDebounced());
        }

        public void UpdateDebounced()
        {
            _instances.ForEach(x => x.Sync());
            WindowInstances.I.Save(_context, _instances);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var metro = sender as MetroWindow;
            if (metro == null)
                return;
            _instances.RemoveAll(x => Equals(x.MetroWindow, metro));
            Update();
        }

        public void MinimiseAll()
        {
            _instances.ForEach(x => x.Minimise());
            Update();
        }

        public void ShowAll()
        {
            _instances.ForEach(x => x.Show());
            Update();
        }

        public void Close(WindowInstanceBase windowInstance)
        {
            var metro = windowInstance as MetroWindowInstance;
            metro?.Close();
        }

        public bool CanSpawn()
        {
            return true;
        }

        public bool CanMinimiseAll()
        {
            return _instances.Count > 0 && _instances.Any(x => !x.IsMinimised);
        }

        public bool CanShowAll()
        {
            return _instances.Count > 0 && _instances.Any(x => x.IsMinimised);
        }

        public void Close(object window)
        {
            var metro = window as MetroWindowInstance;
            metro?.Close();
        }
    }
}