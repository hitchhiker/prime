using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Ui.Wpf.Annotations;

namespace Prime.Ui.Wpf.ViewModel
{
    public class VmBase : ViewModelBase
    {
        public VmBase() : base() { }

        public VmBase(IMessenger messenger) : base(messenger) { }

        private ObjectId _id;
        public ObjectId Id => _id ?? (_id = ObjectId.NewObjectId());

        /// <summary>
        /// A string token, used for messages that uniquely identifies this ViewModel instance.
        /// </summary>
        public string RequesterTokenVm => Id.ToString();

        public Dispatcher UiDispatcher => PrimeWpf.I.UiDispatcher;

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