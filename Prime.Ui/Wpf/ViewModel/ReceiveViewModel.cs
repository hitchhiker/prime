using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Prime.Core;
using SharpDX.Collections;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ReceiveViewModel : DocumentPaneViewModel
    {
        private readonly ScreenViewModel _screen;

        public RelayCommand ClickCommand { get; private set; }

        public ReceiveViewModel(ScreenViewModel screen)
        {
            _screen = screen;
            Services = Networks.I.WalletProviders;
            ServiceSelected = Services.FirstProvider();
            PropertyChanged += ReceiveViewModel_PropertyChanged;
            ClickCommand = new RelayCommand(AddAddress);
            WalletAddresses = new BindingList<WalletAddress>(WalletProvider.GetAll().ToList());
        }

        private void ReceiveViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ServiceSelected))
                ServiceChanged();
        }

        private void ServiceChanged()
        {
            Assets.Clear();
            var assets = PublicContext.I.ExchangeDatas.GetOrCreate(PublicContext.I, ServiceSelected).Assets;

            if (!assets.Contains(AssetSelected))
                AssetSelected = null;

            foreach (var i in assets)
                Assets.Add(i);

            RaisePropertyChanged(()=> Assets);
        }

        private void AddAddress()
        {
            WalletProvider.AddAddress(ServiceSelected, AssetSelected, () =>
            {
                WalletAddresses.Clear();
                foreach (var i in WalletProvider.GetAll())
                    WalletAddresses.Add(i);
            });
        }

        public WalletProvider WalletProvider => UserContext.Current.WalletProvider;

        public IReadOnlyList<IWalletService> Services { get; set; }

        public BindingList<Asset> Assets { get; set; } = new BindingList<Asset>();

        public BindingList<WalletAddress> WalletAddresses { get; set; } = new BindingList<WalletAddress>();

        private IWalletService _serviceSelected;
        public IWalletService ServiceSelected
        {
            get => _serviceSelected;
            set => Set(ref _serviceSelected, value);
        }

        private Asset _assetSelected;
        public Asset AssetSelected
        {
            get => _assetSelected;
            set => Set(ref _assetSelected, value);
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("receive");
        }
    }
}