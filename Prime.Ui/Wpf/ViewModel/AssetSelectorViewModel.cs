using Prime.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class AssetSelectorViewModel : VmBase
    {
        public AssetSelectorViewModel()
        {
            ListAssets = new ObservableCollection<Asset>();
            AllAssetsViewModel = new AllAssetsViewModel();
            _debouncer = new Debouncer();
            //M.RegisterAsync<AssetFoundMessage>(this, AssetFound);
            M.RegisterAsync<AssetAllResponseMessage>(this, RetreiveAllAssets);
            M.SendAsync(new AssetAllRequestMessage(RequesterTokenVm));
        }

        private int _maxPopular = 8;
        private readonly Debouncer _debouncer;

        public AllAssetsViewModel AllAssetsViewModel { get; }
        public ObservableCollection<Asset> ListAssets { get; private set; }

        private Asset _selectedAsset;
        public Asset SelectedAsset
        {
            get => _selectedAsset;
            set => Set(ref _selectedAsset, value);
        }

        private void RetreiveAllAssets(AssetAllResponseMessage m)
        {
            if (m.RequesterToken != RequesterTokenVm)
                return;

            UiDispatcher.Invoke(() =>
            {
                ListAssets.Clear();
                foreach (var i in m.Assets)
                    ListAssets.Add(i);

                InvalidateProperties();
                //msg.Unregister<AssetAllResponseMessage>(this, RetreiveAllAssets);
            });
        }

        //public void PopulateAssets()
        //{
        //_listComboItems = new List<ComboSectionItem>();

        //foreach (var i in popular)
        //    _listComboItems.Add(new ComboSectionItem { Header = "Most Popular", Asset = i });

        //foreach (var i in regular)
        //    _listComboItems.Add(new ComboSectionItem { Header = "More Assets...", Asset = i });

        //ListCollectionView listComboSections = new ListCollectionView(_listComboItems);
        //listComboSections.GroupDescriptions.Add(new PropertyGroupDescription("Header"));
        //}

        private void AssetFound(AssetFoundMessage m)
        {
            UiDispatcher.Invoke(() =>
            {
                ListAssets.Add(m.Asset);
                _debouncer.Debounce(50, o => InvalidateProperties());
            });
        }

        private void InvalidateProperties()
        {
            RaisePropertyChanged(nameof(ListAssets));
        }

        public void Dispose()
        {
            M.UnregisterAsync(this);
        }

        //private List<ComboSectionItem> _listComboItems;

        //public class ComboSectionItem
        //{
        //    public string Header { get; set; }
        //    public Common.Asset Asset { get; set; }
        //}
    }
}
