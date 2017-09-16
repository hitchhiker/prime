using GalaSoft.MvvmLight.Command;
using Prime.Core;
using Prime.Core.Trade;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prime.Ui.Wpf.ViewModel
{
    public class BuySellViewModel : DocumentPaneViewModel
    {
        private readonly ScreenViewModel _model;
        private Random random = new Random();
        public Money BidAsk { get; }
        public Money Units { get; }
        public Money Total { get; }
        public TradeTypeModel Type { get; }
        public TimeInForceModel TimeInForce { get; }
        public ObservableCollection<OrderBookModel> ListOrderBookBuy { get; private set; }
        public ObservableCollection<OrderBookModel> ListOrderBookSell { get; private set; }
        public ObservableCollection<OpenOrdersModel> ListOpenOrders { get; private set; }
        public ObservableCollection<MarketHistoryModel> ListMarketHistory { get; private set; }
        public ObservableCollection<MyOrderHistoryModel> ListMyOrderHistory { get; private set; }
        public ObservableCollection<TradeTypeModel> ListTradeTypes { get; private set; }
        public ObservableCollection<TimeInForceModel> ListTimeInForce { get; private set; }
        
        public BuySellViewModel(ScreenViewModel model)
        {
            _model = model;
            new Task(PopulateDataStructures).Start();
        }
        private void PopulateDataStructures()
        {
            ListOrderBookBuy = new ObservableCollection<OrderBookModel>();
            ListOrderBookSell = new ObservableCollection<OrderBookModel>();
            ListOpenOrders = new ObservableCollection<OpenOrdersModel>();
            ListMarketHistory = new ObservableCollection<MarketHistoryModel>();
            ListMyOrderHistory = new ObservableCollection<MyOrderHistoryModel>();
            ListTradeTypes = new ObservableCollection<TradeTypeModel>
            {
                new TradeTypeModel(0, "Limit"),
                new TradeTypeModel(1, "Conditional")
            };

            ListTimeInForce = new ObservableCollection<TimeInForceModel>
            {
                new TimeInForceModel(0, "Good 'Til Cancelled"),
                new TimeInForceModel(1, "Immediate or Cancel")
            };

            for (int i = 0; i < 5; i++)
            {
                ListOrderBookBuy.Add(new OrderBookModel(new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10 - 0.001) + 0.001), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
                ListOrderBookSell.Add(new OrderBookModel(new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10 - 0.001) + 0.001), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
                ListOpenOrders.Add(new OpenOrdersModel(DateTime.Now, (i % 2 == 0 ? "Good 'Til Cancelled" : "Immediate or Cancel"), (i % 2 == 0 ? "Bid" : "Ask"), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
                ListMarketHistory.Add(new MarketHistoryModel(DateTime.Now, new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), (i % 2 == 0 ? "Buy" : "Sell"), (i % 2 == 0 ? "Bid" : "Ask")));
                ListMyOrderHistory.Add(new MyOrderHistoryModel((i % 2 == 0 ? "Good 'Til Cancelled" : "Immediate or Cancel"), (i % 2 == 0 ? "Bid" : "Ask"), DateTime.Now, DateTime.Now, new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
            }
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("buy sell");
        }
    }
}
