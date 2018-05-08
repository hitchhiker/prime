using System;
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
    public class AllAssetsViewModel : VmBase, IDisposable
    {
        public AllAssetsViewModel()
        {
            if (IsInDesignMode)
                return;

            _debouncer = new Debouncer();

            Context = UserContext.Current;
            SelectedAsset = Context.QuoteAsset;

            M.RegisterAsync<AssetFoundMessage>(this, AssetFound);

            M.RegisterAsync<AssetAllResponseMessage>(this, RetreiveAllAssets);

            M.SendAsync(new AssetAllRequestMessage(RequesterTokenVm));
        }

        public readonly UserContext Context;

        private readonly Debouncer _debouncer;

        private Asset _selectedAsset;

        public BindingList<Asset> Assets { get; private set; } = new BindingList<Asset>();

        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set => SetAfter(ref _selectedAsset, value, x =>
            {
                if (SetAsDefault)
                    Context.QuoteAsset = x;
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

        public void Dispose()
        {
            M.UnregisterAsync(this);
        }
    }
}