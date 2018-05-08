using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Prime.Ui.Wpf.Annotations;

namespace Prime.Ui.Wpf
{
    public class VmBaseControl : UserControl, INotifyPropertyChanged
    {
        public VmBaseControl()
        {
            this.Unloaded += VmBase_Unloaded;
        }

        private void VmBase_Unloaded(object sender, RoutedEventArgs e)
        {
            Timers.ForEach(delegate(Timer x)
            {
                x.Enabled = false;
                x.Dispose();
            });
        }

        protected readonly List<Timer> Timers = new List<Timer>();

        public void GetTimer(double ms, Action elapsed)
        {
            var t = new Timer
            {
                AutoReset = true,
                Interval = ms
            };
            t.Elapsed += delegate { elapsed?.Invoke(); };
            t.Enabled = true;
            Timers.Add(t);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual bool Set<T>(ref T value, T newValue, Func<T, T> preFilter, [CallerMemberName] string propertyName = null)
        {
            if (preFilter!=null && newValue!=null)
                newValue = preFilter.Invoke(newValue);

            if (object.Equals(value, newValue))
                return false;
            
            value = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual bool Set<T>(ref T value, T newValue, [CallerMemberName] string propertyName = null)
        {
            return Set<T>(ref value, newValue, null, propertyName);
        }

        private bool? _isDesignMode;
        public bool IsDesignMode => (bool) (_isDesignMode ?? (_isDesignMode = (bool) DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue));
    }
}