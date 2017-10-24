using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
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
using LiveCharts;
using LiveCharts.Wpf;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;
using System.Drawing;
using Prime.Common.Wallet;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for Portfolio.xaml
    /// </summary>
    public partial class Portfolio
    {
        private readonly DebouncerDispatched _debouncer;

        public Portfolio()
        {
            InitializeComponent();
            _debouncer = new DebouncerDispatched(Dispatcher);
            this.DataContextChanged += Portfolio_DataContextChanged;
            SViewer.PreviewMouseWheel += SViewer_PreviewMouseWheel;

            PortfolioPieChart.DataContext = new PortfolioPieChartViewModel();
        }

        private void SViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void Portfolio_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //_paneViewModel = (DataContext as PortfolioPaneViewModel);
            //_paneViewModel.OnChanged += (_, __) => PortfolioPieChart.UpdatePieChartSeries((PortfolioPaneViewModel)this.DataContext);
        }

        private PortfolioPaneViewModel _paneViewModel;
    } 
}
