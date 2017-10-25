using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LiveCharts;
using LiveCharts.Wpf;
using Prime.Common.Wallet;
using System.Drawing;
using Prime.Common;
using Prime.Ui.Wpf.ViewModel;

namespace Prime.Ui.Wpf.View.Portfolio
{
    /// <summary>
    /// Interaction logic for PortfolioPieChartControl.xaml
    /// </summary>
    public partial class PortfolioPieChartControl : UserControl
    {
        public PortfolioPieChartControl()
        {
            InitializeComponent();
        }
    }

}
