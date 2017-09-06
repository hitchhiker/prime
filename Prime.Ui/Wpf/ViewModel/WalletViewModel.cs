using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Prime.Core;

namespace Prime.Ui.Wpf.ViewModel
{
    public class WalletViewModel : DocumentPaneViewModel
    {
        private readonly ScreenViewModel _screenVm;

        public WalletViewModel(ScreenViewModel screenVm)
        {
            _screenVm = screenVm;
            Addresses = new ObservableCollection<WalletAddress>();
            new Task(PullWallet).Start();
        }

        private void PullWallet()
        {
            var wac = new WalletAddressAssetContext(Asset.Btc, false, UserContext.Current);
            foreach (var network in Networks.I.WalletProviders)
            {
                foreach (var i in network.FetchDepositAddresses(wac))
                    Addresses.Add(i);
            }
        }

        public ObservableCollection<WalletAddress> Addresses { get; private set; }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("wallet");
        }
    }
}
