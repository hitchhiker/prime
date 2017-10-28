using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using Prime.Common;
using Prime.Utility;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AllAssetsViewModel : VmBase
    {
        public AllAssetsViewModel()
        {
            if (IsInDesignMode)
                return;

            _debouncer = new Debouncer();

            Context = UserContext.Current;
            SelectedAsset = Context.QuoteAsset;
            var msg = DefaultMessenger.I.Default;

            msg.Register<AssetFoundMessage>(this, AssetFound);

            msg.Register<AssetAllResponseMessage>(this, RetreiveAllAssets);

            msg.Send(new AssetAllRequestMessage(RequesterTokenVm));
        }

        public readonly UserContext Context;

        private readonly Debouncer _debouncer;

        private Asset _selectedAsset;

        public BindingList<Asset> Assets { get; private set; } = new BindingList<Asset>();

        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set => Set(ref _selectedAsset, value, x =>
            {
                if (SetAsDefault)
                    return Context.QuoteAsset = x;
                return x;
            });
        }

        public bool SetAsDefault { get; set; }

        public AddressBoxModel AddressBoxModel { get; set; }

        private void AssetFound(AssetFoundMessage m)
        {
            UiDispatcher.Invoke(() =>
            {
                Assets.Add(m.Asset);
                _debouncer.Debounce(50, o => InvalidateProperties());
            });
        }

        private void RetreiveAllAssets(AssetAllResponseMessage m)
        {
            if (m.RequesterToken != RequesterTokenVm)
                return;

            var msg = DefaultMessenger.I.Default;

            UiDispatcher.Invoke(() =>
            {
                Assets.Clear();
                foreach (var i in m.Assets)
                    Assets.Add(i);

                InvalidateProperties();
                msg.Unregister<AssetAllResponseMessage>(this, RetreiveAllAssets);
            });
        }

        private void InvalidateProperties()
        {
            RaisePropertyChanged(nameof(Assets));
            //CollectionViewSource.GetDefaultView(Assets).Refresh();
        }
    }
}