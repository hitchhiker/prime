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
        private readonly ScreenViewModel _screen;

        public RelayCommand ClickCommand { get; private set; }

        public ReceiveViewModel(ScreenViewModel screen)
        {
            _screen = screen;
            Services = Networks.I.WalletProviders;
            ServiceSelected = Services.FirstProvider();
            ServiceChanged(false);

            ClickCommand = new RelayCommand(AddAddress);
            WalletAddresses = new BindingList<WalletAddress>(WalletProvider.GetAll().ToList());

            M.RegisterAsync<WalletAddressResponseMessage>(this, UiDispatcher, m =>
            {
                WalletAddresses.Add(m.Address);
            });

            M.RegisterAsync<WalletAllResponseMessage>(this, UiDispatcher, m =>
            {
                foreach (var a in m.Addresses)
                    WalletAddresses.Add(a);
            });
        }
        
        private void ServiceChanged(bool raiseChanged = true)
        {
            new Task(() =>
            {
                var assets = PublicContext.I.ExchangeDatas.GetOrCreate(PublicContext.I, ServiceSelected).Assets;

                UiDispatcher.Invoke(() =>
                {
                    Assets.Clear();

                    if (!assets.Contains(AssetSelected))
                        AssetSelected = null;

                    foreach (var i in assets)
                        Assets.Add(i);

                    if (raiseChanged)
                        RaisePropertyChanged(() => Assets);
                });
            }).Start();
        }

        private void AddAddress()
        {
            M.SendAsync(new WalletAddressRequestMessage(ServiceSelected.Network, AssetSelected));
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