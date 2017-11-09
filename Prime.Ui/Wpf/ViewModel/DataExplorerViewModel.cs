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

            FilterSearchCommand = new RelayCommand(() =>
            {
                CollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ListDataExplorerItems);

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
