using Prime.Core;
using Prime.Core.Trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;

namespace Prime.Ui.Wpf.ViewModel.Trading
{
    public class BuyViewModel
    {
        private readonly BuySellViewModel _model;

        public Money Bid { get; set; }
        public Money Units { get; set; }
        public Money Total { get; set; }
        public TradeTypeModel Type { get; set; }
        public TimeInForceModel TimeInForce { get; set; }

        public BindingList<TradeTypeModel> ListTradeTypes { get; private set; }
        public BindingList<TimeInForceModel> ListTimeInForce { get; private set; }

        public RelayCommand BuyCommand { get; }

        public BuyViewModel(BuySellViewModel model)
        {
            _model = model;

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
    }
}
