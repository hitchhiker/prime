using GalaSoft.MvvmLight.Command;
using Prime.Common;
using Prime.Common.Trade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Ui.Wpf.ViewModel.Trading
{
    public class SellViewModel
    {
        public Money Ask { get; set; }
        public Money Units { get; set; }
        public Money Total { get; set; }
        public TradeTypeModel Type { get; set; }
        public TimeInForceModel TimeInForce { get; set; }

        public BindingList<TradeTypeModel> ListTradeTypes { get; private set; }
        public BindingList<TimeInForceModel> ListTimeInForce { get; private set; }

        public RelayCommand SellCommand { get; }

        public SellViewModel()
        {
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

            SellCommand = new RelayCommand(() =>
            {
                // do vmodel stuff here (in method) for when the button is clicked.
            });
        }
    }
}
