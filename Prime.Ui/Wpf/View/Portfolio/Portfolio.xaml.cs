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
using Prime.Core;
using LiveCharts;
using LiveCharts.Wpf;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;
using System.Drawing;
using Prime.Core.Wallet;
using Prime.Ui.Wpf.ViewModel;
using Prime.Utility;

namespace Prime.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for Portfolio.xaml
    /// </summary>
    public partial class Portfolio
    {
        private readonly Debouncer _debouncer;

        public Portfolio()
        {
            InitializeComponent();
            _debouncer = new Debouncer(Dispatcher);
            this.DataContextChanged += Portfolio_DataContextChanged;
            SViewer.PreviewMouseWheel += SViewer_PreviewMouseWheel;
        }

        private void SViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void Portfolio_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _paneViewModel = (DataContext as PortfolioPaneViewModel);
            _paneViewModel.OnChanged += (_, __) => UpdatePieChartSeries();
        }

        private readonly object _lock = new object();

        private PortfolioPaneViewModel _paneViewModel;
        private readonly ConcurrentDictionary<Network, decimal> _lastPercentages = new ConcurrentDictionary<Network, decimal>();
        
        private void UpdatePieChartSeries()
        {
            lock (_lock)
            {
                var items = _paneViewModel.PortfolioInfoObservable.ToList();

                foreach (var i in items)
                {
                    var percentage = i.Percentage;
                    var name = i.Network.Name;

                    var lp = _lastPercentages.GetOrAdd(i.Network, percentage);

                    if (Math.Abs(lp - percentage) < 1)
                        continue;

                    _lastPercentages.AddOrUpdate(i.Network, percentage, (_, __) => percentage);

                    var e = Pie.Series.FirstOrDefault(x => x.Title == name);
                    var series = GetSeries(i);

                    if (e != null)
                        e.Values = series.Values;
                    else
                        Pie.Series.Add(series);

                }
            }
        }

        private static PieSeries GetSeries(PortfolioInfoItem item)
        {
            return new PieSeries
            {
                Title = item.Network.Name,
                Fill = StringColor(item.Network.Name),
                Values = new ChartValues<double>() {item.TotalConvertedAssetValue.ToDouble(null)},
                DataLabels = true,
                LabelPosition = PieLabelPosition.InsideSlice,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#555"),
                LabelPoint = c => c.Participation>0.05 ? $"{Math.Round(c.Participation, 2):P}" : null
            };
        }

        private static System.Windows.Media.Brush StringColor(string str)
        {
            var names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            var randomColorName = names[(uint)str.GetHashCode() % names.Length];
            return (SolidColorBrush) new BrushConverter().ConvertFromString(randomColorName.ToString());
        }
    } 
}
