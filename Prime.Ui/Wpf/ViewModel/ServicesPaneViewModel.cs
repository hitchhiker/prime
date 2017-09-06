using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Core.Wallet;
using GalaSoft.MvvmLight.Messaging;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ServicesPaneViewModel : DocumentPaneViewModel
    {
        private readonly IMessenger _messenger;
        private readonly ScreenViewModel _model;

        public ServicesPaneViewModel(IMessenger messenger, ScreenViewModel model)
        {
            _messenger = messenger;
            _model = model;
            Populate();
        }

        public ObservableCollection<ServiceLineItem> ServicesObservable { get; private set; } = new ObservableCollection<ServiceLineItem>();

        private bool _hasApiKey = true;
        public bool HasApiKey { get => _hasApiKey; set => ESet(ref _hasApiKey, value); }

        private void Populate()
        {
            ServicesObservable.Clear();
            var q = Networks.I.Providers.AsEnumerable();

            if (HasApiKey)
                q = q.FilterType<INetworkProvider, INetworkProviderPrivate>();

            foreach (var i in q.OrderBy(x=>x.Network.Name))
                ServicesObservable.Add(new ServiceLineItem(i));
        }

        protected bool ESet<T>(ref T value, T newValue, [CallerMemberName] string propertyName = null)
        {
            var changed = base.Set(ref value, newValue, propertyName);
            if (!changed)
                return false;

            Populate();
            RaisePropertyChanged(nameof(ServicesObservable));
            return true;
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("services");
        }
    }
}
