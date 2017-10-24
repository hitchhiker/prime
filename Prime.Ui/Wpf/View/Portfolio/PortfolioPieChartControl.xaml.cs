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
            UpdatePieChartSeries();
        }

        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<Network, decimal> _lastPercentages = new ConcurrentDictionary<Network, decimal>();

        public void UpdatePieChartSeries()
        {
            //This would be called when a message is received from the PortfolioPieChartViewModel, that a new exchange and percentage value has been added.

            ObservableCollection<PortfolioInfoItem> listPieChartItems = (ObservableCollection<PortfolioInfoItem>)this.DataContext;

            lock (_lock)
            {
                //var items = portfolioPaneViewModel.PortfolioInfoObservable.ToList();

                //foreach (var i in items)
                foreach (var i in listPieChartItems)
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
                Values = new ChartValues<double>() { item.TotalConvertedAssetValue.ToDouble(null) },
                DataLabels = true,
                LabelPosition = PieLabelPosition.InsideSlice,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#555"),
                LabelPoint = c => c.Participation > 0.05 ? $"{Math.Round(c.Participation, 2):P}" : null
            };
        }

        private static System.Windows.Media.Brush StringColor(string str)
        {
            var names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            var randomColorName = names[(uint)str.GetHashCode() % names.Length];
            return (SolidColorBrush)new BrushConverter().ConvertFromString(randomColorName.ToString());
        }
    }

}
