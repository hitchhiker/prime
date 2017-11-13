using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Prime.Common;
using Prime.Utility;
using System.Windows.Data;
using Prime.Ui.Wpf.ViewModel.Ticker;

namespace Prime.Ui.Wpf.ViewModel
{
    public class DataExplorerViewModel : DocumentPaneViewModel
    {
        public DataExplorerViewModel()
        {
            _debouncer = new DebouncerDispatched(UiDispatcher);
            M.RegisterAsync<AssetPairAllResponseMessage>(this, RetreiveAllAssets);
            M.SendAsync(new AssetPairAllRequestMessage());

            _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(ListDataExplorerItems);
            _collectionView.Filter = GetFilter;

            FilterSearchCommand = new RelayCommand(AddRequestDebounced);
        }

        private readonly DebouncerDispatched _debouncer;
        private readonly CollectionView _collectionView;

        private void AddRequestDebounced()
        {
            _debouncer.Debounce(500, o => FilterSearchDebounced());
        }

        private bool GetFilter(object model)
        {
            if (string.IsNullOrWhiteSpace(FilterText))
                return true;

            if (!(model is DataExplorerItemModel m))
                return false;

            return m.Title.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase);
        }

        private void FilterSearchDebounced()
        {
            _collectionView.Refresh();
        }

        public TickerContainerViewModel TickerContainerViewModel { get; } = new TickerContainerViewModel();
        public ObservableCollection<DataExplorerItemModel> ListDataExplorerItems { get; } = new ObservableCollection<DataExplorerItemModel>();

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
