using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prime.Common;
using FirstFloor.ModernUI.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;
using Prime.Ui.Wpf.ExtensionMethods;
using GalaSoft.MvvmLight.Command;
using Prime.Common.Exchange.Model;
using Prime.Ui.Wpf.View.Trade;
using Prime.Ui.Wpf.ViewModel.Trading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class MarketControlViewModel
    {
        public decimal ChangePerc { get; }
        public string IconPath { get; }
        public string Name { get; }
        public Money Buy { get ; }
        public Money Sell { get; }

        public RelayCommand BuyCommand { get; }
        public RelayCommand SellCommand { get; }

        public MarketControlViewModel()
        {
        }

        public MarketControlViewModel(MarketsDiscoveryItemModel model)
        {
            ChangePerc = model.ChangePerc;
            IconPath = model.IconPath;
            Name = model.Name;
            Buy = model.Buy;
            Sell = model.Sell;
        }
    }
}
