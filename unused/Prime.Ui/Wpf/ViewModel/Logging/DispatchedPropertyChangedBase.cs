using System;
using System.ComponentModel;
using System.Windows;

namespace Prime.Ui.Wpf.ViewModel
{
    /// <summary>
    /// https://stackoverflow.com/a/16745054/1318333
    /// </summary>
    public class DispatchedPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }
}