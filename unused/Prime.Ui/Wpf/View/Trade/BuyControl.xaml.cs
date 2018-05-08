using System.Windows;
using System.Windows.Controls;

namespace Prime.Ui.Wpf.View.Trade
{
    /// <summary>
    /// Interaction logic for BuyControl.xaml
    /// </summary>
    public partial class BuyControl : UserControl
    {
        public BuyControl()
        {
            InitializeComponent();
        }

        private void CmbBuyType_Loaded(object sender, RoutedEventArgs e)
        {
            CmbBuyType.SelectedIndex = 0;
        }

        private void CmbBuyTimeInForce_Loaded(object sender, RoutedEventArgs e)
        {
            CmbBuyTimeInForce.SelectedIndex = 0;
        }
    }
}
