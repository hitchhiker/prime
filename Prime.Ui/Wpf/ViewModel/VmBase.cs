using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LiteDB;
using Prime.Ui.Wpf.Annotations;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class VmBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObjectId _id;
        public ObjectId Id => _id ?? (_id = ObjectId.NewObjectId());

        /// <summary>
        /// A string token, used for messages that uniquely identifies this ViewModel instance.
        /// </summary>
        public string RequesterTokenVm => Id.ToString();

        public Dispatcher UiDispatcher => PrimeWpf.I.UiDispatcher;

        /// <summary>
        /// Shortcut to the current IMessenger instance.
        ///  </summary>
        public IMessenger M => Messenger.Default;

        [NotifyPropertyChangedInvocator]
        protected virtual bool Set<T>(ref T value, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, newValue))
                return false;

            value = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual bool Set<T>(ref T value, T newValue, Func<T, T> preFilter, [CallerMemberName] string propertyName = null)
        {
            newValue = preFilter(newValue);
            return Set(ref value, newValue, propertyName);
        }

        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;
            
            RaisePropertyChanged(propertyExpression);
            return true;
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual bool SetAfter<T>(ref T value, T newValue, Action<T> after, [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, newValue))
                return false;

            value = newValue;
            RaisePropertyChanged(propertyName);

            new Task(() => after.Invoke(newValue)).Start();

            return true;
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));
            
            if (!(propertyExpression.Body is MemberExpression body))
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            
            var property = body.Member as PropertyInfo;

            if (property == null)
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            
            return property.Name;
        }
    
        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged(GetPropertyName(propertyExpression));
        }

        [NotifyPropertyChangedInvocator]
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsInDesignMode => IsInDesignModeStatic;

        private static bool? _isInDesignMode;
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (_isInDesignMode.HasValue)
                    return _isInDesignMode.Value;

                var prop = DesignerProperties.IsInDesignModeProperty;
                _isInDesignMode = (bool) DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return _isInDesignMode.Value;
            }
        }
    }
}