using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using Prime.Core;
using Prime.Utility;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AllAssetsViewModel : VmBase
    {
        public AllAssetsViewModel() : this(null) { }

        public AllAssetsViewModel(ScreenViewModel screenViewModel)
        {
            _screenViewModel = screenViewModel;
            _debouncer = new Debouncer();
            "USD".ToAssetRaw();
            "EUR".ToAssetRaw();

            Context = UserContext.Current;
            SelectedBaseAsset = Context.QuoteAsset;

            UpdateAssets();

            Core.Assets.I.OnAssetsUpdated += (_, e) => _debouncer.Debounce(100, o => UpdateAssets());
        }

        public bool SetAsDefault { get; set; }

        private readonly ScreenViewModel _screenViewModel;
        private readonly Debouncer _debouncer;

        public AddressBoxModel AddressBoxModel { get; set; }

        private void UpdateAssets()
        {
            var currentAsssets = Core.Assets.I.Cached().Where(x => !Equals(x, Asset.None)).OrderBy(x => x.ShortCode).ToList();

            foreach (var i in Assets.Except(currentAsssets))
                Assets.Remove(i);

            foreach (var i in currentAsssets.Except(Assets))
                Assets.Add(i);

            RaisePropertyChanged(nameof(Assets));
            CollectionViewSource.GetDefaultView(Assets).Refresh();
        }

        public readonly UserContext Context;
        private Asset _selectedBaseAsset;

        public Asset SelectedBaseAsset
        {
            get => _selectedBaseAsset;
            set => Set(ref _selectedBaseAsset, value, x =>
            {
                if (SetAsDefault)
                    return Context.QuoteAsset = x;
                return x;
            });
        }


        public BindingList<Asset> Assets { get; private set; } = new BindingList<Asset>();
    }
}