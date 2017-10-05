using GalaSoft.MvvmLight.Command;
using Prime.Core;
using Prime.Core.Trade;
using Prime.Ui.Wpf.ViewModel.Trading;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prime.Ui.Wpf.ViewModel.Trading
{
    public class BuySellViewModel : DocumentPaneViewModel
    {
        private readonly ScreenViewModel _model;

        public BuyViewModel BuyViewModel { get; }
        public SellViewModel SellViewModel { get; }
        public OrderBookViewModel OrderBookBuyViewModel { get; }
        public OrderBookViewModel OrderBookSellViewModel { get; }
        public OpenOrdersViewModel OpenOrdersViewModel { get; }
        public MarketHistoryViewModel MarketHistoryViewModel { get; }
        public MyOrderHistoryViewModel MyOrderHistoryViewModel { get; }
        
        public BuySellViewModel(ScreenViewModel model)
        {
            _model = model;

            BuyViewModel = new BuyViewModel(this);
            SellViewModel = new SellViewModel(this);
            OrderBookBuyViewModel = new OrderBookViewModel(this);
            OrderBookSellViewModel = new OrderBookViewModel(this);
            OpenOrdersViewModel = new OpenOrdersViewModel(this);
            MarketHistoryViewModel = new MarketHistoryViewModel(this);
            MyOrderHistoryViewModel = new MyOrderHistoryViewModel(this);
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("buy sell");
        }
    }
}
