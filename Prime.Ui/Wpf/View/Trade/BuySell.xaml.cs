using Prime.Ui.Wpf.ViewModel;
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

namespace Prime.Ui.Wpf.View.Trade
{
    /// <summary>
    /// Interaction logic for BuySell.xaml
    /// </summary>
    public partial class BuySell : UserControl
    {
        public BuySell()
        {
            InitializeComponent();
        }

        private void CmbBuyType_Loaded(object sender, RoutedEventArgs e)
        {
            cmbBuyType.SelectedIndex = 0;
        }

        private void CmbBuyTimeInForce_Loaded(object sender, RoutedEventArgs e)
        {
            cmbBuyTimeInForce.SelectedIndex = 0;
        }

        private void CmbSellTimeInForce_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSellTimeInForce.SelectedIndex = 0;
        }

        private void CmbSellType_Loaded(object sender, RoutedEventArgs e)
        {
            cmbSellType.SelectedIndex = 0;
        }
    }
}
