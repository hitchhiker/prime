using GalaSoft.MvvmLight.Messaging;
using Prime.Core;
using Prime.Core.Trade;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class BuySellViewModel : DocumentPaneViewModel
    {
        private readonly ScreenViewModel _model;
        private Random random = null;
        public ObservableCollection<OrderBookModel> ListOrderBookBuy { get; private set; }
        public ObservableCollection<OrderBookModel> ListOrderBookSell { get; private set; }
        public ObservableCollection<OpenOrdersModel> ListOpenOrders { get; private set; }
        public ObservableCollection<MarketHistoryModel> ListMarketHistory { get; private set; }
        public ObservableCollection<MyOrderHistoryModel> ListMyOrderHistory { get; private set; }

        public BuySellViewModel(ScreenViewModel model)
        {
            _model = model;
            random = new Random();
            ListOrderBookBuy = new ObservableCollection<OrderBookModel>();
            ListOrderBookSell = new ObservableCollection<OrderBookModel>();
            ListOpenOrders = new ObservableCollection<OpenOrdersModel>();
            ListMarketHistory = new ObservableCollection<MarketHistoryModel>();
            ListMyOrderHistory = new ObservableCollection<MyOrderHistoryModel>();
            new Task(PopulateTables).Start();
        }

        private void PopulateTables()
        {
            for (int i = 0; i < 5; i++)
            {
                ListOrderBookBuy.Add(new OrderBookModel((decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10 - 0.001) + 0.001), (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000)));
                ListOrderBookSell.Add(new OrderBookModel((decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10 - 0.001) + 0.001), (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000)));
                ListOpenOrders.Add(new OpenOrdersModel((decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (i % 2 == 0 ? "Pending" : "Completed")));
                ListMarketHistory.Add(new MarketHistoryModel(DateTime.Now, (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (i % 2 == 0 ? "Buy" : "Sell"), (i % 2 == 0 ? "Bid" : "Ask")));
                ListMyOrderHistory.Add(new MyOrderHistoryModel((decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000), (decimal)(random.NextDouble() * (10000 - 1000) + 1000)));
            }
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("buy sell");
        }
    }
}
