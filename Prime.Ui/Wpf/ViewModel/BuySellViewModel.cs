using GalaSoft.MvvmLight.Command;
using Prime.Core;
using Prime.Core.Trade;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public BindingList<MyOrderHistoryModel> ListMyOrderHistory { get; private set; }
        public BindingList<TradeTypeModel> ListTradeTypes { get; private set; }
        public BindingList<TimeInForceModel> ListTimeInForce { get; private set; }

        public OrderBookViewModel OrderBookViewModel { get; }
        public OpenOrdersViewModel OpenOrdersViewModel { get; }
        public MarketHistoryViewModel MarketHistoryViewModel { get; }
        public MyOrderHistoryViewModel MyOrderHistoryViewModel { get; }

        public RelayCommand BuyCommand { get; private set; }

        public BuySellViewModel(ScreenViewModel model)
        {
            _model = model;

            OrderBookViewModel = new OrderBookViewModel(this);
            OpenOrdersViewModel = new OpenOrdersViewModel(this);
            MarketHistoryViewModel = new MarketHistoryViewModel(this);
            MyOrderHistoryViewModel = new MyOrderHistoryViewModel(this);

            ListTradeTypes = new BindingList<TradeTypeModel>
            {
                new TradeTypeModel(0, "Limit"),
                new TradeTypeModel(1, "Conditional")
            };

            ListTimeInForce = new BindingList<TimeInForceModel>
            {
                new TimeInForceModel(0, "Good 'Til Cancelled"),
                new TimeInForceModel(1, "Immediate or Cancel")
            };

            BuyCommand = new RelayCommand(() =>
            {
                // do vmodel stuff here (in method) for when the button is clicked.
            });
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("buy sell");
        }
    }
}
