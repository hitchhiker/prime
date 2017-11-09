using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Prime.Common;
using Prime.Utility;
using System.Windows.Data;

namespace Prime.Ui.Wpf.ViewModel
{
    public class DataExplorerViewModel : DocumentPaneViewModel
    {
        public DataExplorerViewModel()
        {
            ListDataExplorerItems = new ObservableCollection<DataExplorerItemModel>();
            M.RegisterAsync<AssetPairAllResponseMessage>(this, RetreiveAllAssets);
            M.SendAsync(new AssetPairAllRequestMessage());

            _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(ListDataExplorerItems);

            _collectionView.Filter = GetFilter;

            FilterSearchCommand = new RelayCommand(FilterSearch);
        }

        private readonly CollectionView _collectionView;

        private bool GetFilter(object model)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
                return true;

            if (!(model is DataExplorerItemModel m))
                return false;

            return m.Title.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);
        }

        private void FilterSearch()
        {
            _collectionView.Refresh();
        }

        public ObservableCollection<DataExplorerItemModel> ListDataExplorerItems { get; private set; }

        public string FilterText { get; set; }

        public RelayCommand FilterSearchCommand { get; }

        private void RetreiveAllAssets(AssetPairAllResponseMessage m)
        {
            UiDispatcher.Invoke(() =>
            {
                ListDataExplorerItems.Clear();

                foreach (var currentAssetPair in m.Pairs)
                    ListDataExplorerItems.Add(new DataExplorerItemModel(currentAssetPair.Asset1.ShortCode + " -> " + currentAssetPair.Asset2.ShortCode, currentAssetPair));

                M.Unregister<AssetPairAllResponseMessage>(this, RetreiveAllAssets);
            });
        }
        
        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("data explorer");
        }

        public override void Dispose()
        {
            M.UnregisterAsync(this);
            base.Dispose();
        }
    }
}
