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
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            ListDataExplorerItems = new ObservableCollection<DataExplorerItemModel>();
            M.RegisterAsync<AssetPairAllResponseMessage>(this, RetreiveAllAssets);
            M.SendAsync(new AssetPairAllRequestMessage());

            FilterSearchCommand = new RelayCommand(() =>
            {
                var itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(ListDataExplorerItems);

                itemsViewOriginal.Filter = ((dataExplorerItemModel) =>
                {
                    if (string.IsNullOrWhiteSpace(FilterText)) return true;
                    return ((DataExplorerItemModel)dataExplorerItemModel).Title.IndexOf(FilterText,
                               StringComparison.InvariantCultureIgnoreCase) >= 0;
                });
            });
        }
        
        public ObservableCollection<DataExplorerItemModel> ListDataExplorerItems { get; private set; }

        public string FilterText { get; set; }

        public RelayCommand FilterSearchCommand { get; }

        private void RetreiveAllAssets(AssetPairAllResponseMessage m)
        {
            var msg = DefaultMessenger.I.Default;

            UiDispatcher.Invoke(() =>
            {
                ListDataExplorerItems.Clear();

                foreach (var currentAssetPair in m.Pairs)
                    ListDataExplorerItems.Add(new DataExplorerItemModel(currentAssetPair.Asset1.ShortCode + " -> " + currentAssetPair.Asset2.ShortCode, currentAssetPair));

                msg.Unregister<AssetPairAllResponseMessage>(this, RetreiveAllAssets);
            });
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

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
