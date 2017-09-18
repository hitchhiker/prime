using System;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Prime.Ui.Wpf.Annotations;

namespace Prime.Ui.Wpf.ViewModel
{
    public class VmBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public VmBase() : base() { }

        public VmBase(IMessenger messenger) : base(messenger) { }

        [NotifyPropertyChangedInvocator]
        protected virtual bool Set<T>(ref T value, T newValue, Func<T, T> preFilter, [CallerMemberName] string propertyName = null)
        {
            if (preFilter != null && newValue != null)
                newValue = preFilter.Invoke(newValue);

            if (object.Equals(value, newValue))
                return false;

            value = newValue;
            ((ObservableObject) this).RaisePropertyChanged(propertyName);
            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual bool SetAfter<T>(ref T value, T newValue, Action<T> after, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(value, newValue))
                return false;

            value = newValue;
            ((ObservableObject)this).RaisePropertyChanged(propertyName);
            after.Invoke(newValue);
            return true;
        }
    }
}