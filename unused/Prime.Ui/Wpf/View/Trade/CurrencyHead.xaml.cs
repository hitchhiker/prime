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
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for CurrencyHead.xaml
    /// </summary>
    public partial class CurrencyHead
    {
        public CurrencyHead()
        {
            InitializeComponent();
        }

        public void Bind()
        {
            var dc = this.DataContext as LiveChartOhclViewModel;
            if (dc == null)
                return;

            var a = dc.AssetPair.Asset1;
            var i = a.AssetInfo;

            Symbol.Source = i.LogoBitmap;
            Desc.Text = i.FullName;

            if (!string.IsNullOrWhiteSpace(i.BackgroundColour))
                SymbolBox.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(i.BackgroundColour);
        }
    }
}
