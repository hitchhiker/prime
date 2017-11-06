using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Prime.Common;
using Prime.Common.Exchange.Model;

namespace Prime.Ui.Wpf.ViewModel
{
    public class DataExplorerViewModel : DocumentPaneViewModel
    {
        public DataExplorerViewModel()
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            new Task(PopulateGrid).Start();
        }
        
        public BindingList<DataExplorerItemModel> ListDataExplorerItems { get; private set; }

        private void PopulateGrid()
        {
            ListDataExplorerItems = new BindingList<DataExplorerItemModel>()
            {
                new DataExplorerItemModel("BTC -> USD",new AssetPair(Assets.I.GetRaw("BTC"),Assets.I.GetRaw("USD"))),
                new DataExplorerItemModel("ETH -> USD",new AssetPair(Assets.I.GetRaw("ETH"),Assets.I.GetRaw("USD"))),
                new DataExplorerItemModel("BTC -> EUR",new AssetPair(Assets.I.GetRaw("BTC"),Assets.I.GetRaw("EUR"))),
                new DataExplorerItemModel("BTC -> ETH",new AssetPair(Assets.I.GetRaw("BTC"),Assets.I.GetRaw("ETH"))),
            };
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("data explorer");
        }
    }
}
