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
    /// Interaction logic for SellControl.xaml
    /// </summary>
    public partial class SellControl : UserControl
    {
        public SellControl()
        {
            InitializeComponent();
        }

        private void CmbSellTimeInForce_Loaded(object sender, RoutedEventArgs e)
        {
            CmbSellTimeInForce.SelectedIndex = 0;
        }

        private void CmbSellType_Loaded(object sender, RoutedEventArgs e)
        {
            CmbSellType.SelectedIndex = 0;
        }
    }
}
