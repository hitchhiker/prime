using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Prime.Core;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class ReceiveViewModel : DocumentPaneViewModel
    {
        public RelayCommand ClickCommand { get; private set; }

        public ReceiveViewModel()
        {
            if (IsInDesignMode)
                return;

            var uc = UserContext.Current;

            ClickCommand = new RelayCommand(AddAddress);

            M.RegisterAsync<WalletAddressResponseMessage>(this, uc.Token, UiDispatcher, m =>
            {
                WalletAddresses.Add(m.Address);
            });

            M.RegisterAsync<WalletAllResponseMessage>(this, uc.Token, UiDispatcher, m =>
            {
                foreach (var a in m.Addresses)
                    WalletAddresses.Add(a);
            });

            M.RegisterAsync<AssetNetworkResponseMessage>(this, UiDispatcher, m =>
            {
                UpdateAssets(m.Assets);
            });

            Services = Networks.I.WalletProviders;
            ServiceSelected = Services.FirstProvider();

            ServiceChanged();
        }
        
        private void ServiceChanged()
        {
            UiDispatcher.Invoke(delegate
            {
                Assets.Clear();

                if (ServiceSelected != null)
                    M.SendAsync(new AssetNetworkRequestMessage(ServiceSelected?.Network));
            });
        }

        private void UpdateAssets(IReadOnlyList<Asset> assets)
        {
            if (!assets.Contains(AssetSelected))
                AssetSelected = null;

            foreach (var i in assets)
                Assets.Add(i);

            RaisePropertyChanged(() => Assets);

            if (AssetSelected == null)
                AssetSelected = Assets.EqualOrPegged(UserContext.Current.QuoteAsset) ?? Assets.FirstOrDefault();
        }

        private void AddAddress()
        {
            M.SendAsync(new WalletAddressRequestMessage(ServiceSelected.Network, AssetSelected), UserContext.Current.Token);
        }

        public WalletProvider WalletProvider => UserContext.Current.WalletProvider;

        public IReadOnlyList<IWalletService> Services { get; set; }

        public BindingList<Asset> Assets { get; set; } = new BindingList<Asset>();

        public BindingList<WalletAddress> WalletAddresses { get; set; } = new BindingList<WalletAddress>();

        private IWalletService _serviceSelected;
        public IWalletService ServiceSelected
        {
            get => _serviceSelected;
            set => SetAfter(ref _serviceSelected, value, v => ServiceChanged());
        }

        private Asset _assetSelected;
        public Asset AssetSelected
        {
            get => _assetSelected;
            set => Set(ref _assetSelected, value);
        }

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("receive");
        }

        public override void Dispose()
        {
            M.UnregisterAsync(this);
            base.Dispose();
        }
    }
}