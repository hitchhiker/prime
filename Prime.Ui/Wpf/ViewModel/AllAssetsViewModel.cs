using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using Prime.Core;
using SharpDX.Collections;
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
            _debounceDispatcher = new DebounceDispatcher();
            "USD".ToAssetRaw();
            "EUR".ToAssetRaw();

            Context = UserContext.Current;
            BaseAsset = Context.BaseAsset;

            UpdateAssets();

            Assets.I.OnAssetsUpdated += (_, e) => _debounceDispatcher.Debounce(100, o => UpdateAssets());
        }

        private readonly ScreenViewModel _screenViewModel;
        private readonly DebounceDispatcher _debounceDispatcher;

        public AddressBoxModel AddressBoxModel { get; set; }

        private void UpdateAssets()
        {
            var ast = Assets.I.Cached().Where(x => !Equals(x, Asset.None)).OrderBy(x => x.ShortCode);

            KnownAssetsObservable.Clear();
            foreach (var a in ast)
                KnownAssetsObservable.Add(a);

            RaisePropertyChanged(nameof(KnownAssetsObservable));
            CollectionViewSource.GetDefaultView(KnownAssetsObservable).Refresh();
        }

        public readonly UserContext Context;
        private Asset _baseAsset;

        public Asset BaseAsset
        {
            get => _baseAsset;
            set => Set(ref _baseAsset, value, x => { return Context.BaseAsset = x; });
        }

        public ObservableCollection<Asset> KnownAssetsObservable { get; private set; } = new ObservableCollection<Asset>();
    }
}