using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Prime.Core.Annotations;

namespace Prime.Core
{
    public abstract class NotifierBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual bool Set<T>(ref T value, T newValue, Func<T, T> preFilter, [CallerMemberName] string propertyName = null)
        {
            if (preFilter != null && newValue != null)
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
    }
}