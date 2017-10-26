using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Prime.Common.Wallet;
using Prime.Utility;
using Brush = System.Windows.Media.Brush;

namespace Prime.Ui.Wpf.ViewModel
{
    public class PortfolioPieChartViewModel : VmBase, IDisposable
    {
        public PortfolioPieChartViewModel()
        {
            _context = UserContext.Current;
            new Task(PopulateTestData).Start();
            SeriesPieChartItems = new SeriesCollection();
            M.RegisterAsync<PortfolioResultsMessage>(this, UserContext.Current.Token, PortfolioResultsMessage);
        }

        private Money? _lastTotal;

        private void PortfolioResultsMessage(PortfolioResultsMessage m)
        {
            if (!m.IsConversionComplete)
                return;

            if (_lastTotal != null && !_lastTotal.Value.IsWithinPercentage(m.Total, 1))
                return;

            _lastTotal = m.Total;

            UiDispatcher.Invoke(() => UpdatePieChart(m));
        }

        private void UpdatePieChart(PortfolioResultsMessage m)
        {
            UiDispatcher.Invoke(() =>
                {
                    lock (_lock)
                    {
                        foreach (var i in m.NetworkItems)
                        {
                            var name = i.Network.Name;

                            var e = SeriesPieChartItems.FirstOrDefault(x => x.Title == name);

                            var seriesItem = new PieSeries
                            {
                                Title = i.Network.Name,
                                Fill = StringColor(name),
                                Values = new ChartValues<double>() {(double) i.ConvertedTotal.ToDecimalValue()},
                                DataLabels = true,
                                LabelPosition = PieLabelPosition.InsideSlice,
                                Foreground = (SolidColorBrush) new BrushConverter().ConvertFromString("#555"),
                                LabelPoint = c => c.Participation > 0.05 ? $"{Math.Round(c.Participation, 2):P}" : null
                            };

                            if (e != null)
                                e.Values = new ChartValues<double>() {(double)i.ConvertedTotal.ToDecimalValue()};
                            else
                                SeriesPieChartItems.Add(seriesItem);
                        }
                    }
                }
            );
        }

        private readonly object _lock = new object();

        public SeriesCollection SeriesPieChartItems { get; }

        private void PopulateTestData()
        {
            UiDispatcher.Invoke(() =>
            {
                DateTime timestamp = DateTime.UtcNow;

                //PortfolioDistributionChangedMessage message1 = new PortfolioDistributionChangedMessage(UserContext.Current.Id, 200, 20, timestamp, new PortfolioInfoItem(new Network("Poloniex")));
                //PortfolioDistributionChangedMessage message2 = new PortfolioDistributionChangedMessage(UserContext.Current.Id, 300, 30, timestamp, new PortfolioInfoItem(new Network("Bittrex")));
                //PortfolioDistributionChangedMessage message3 = new PortfolioDistributionChangedMessage(UserContext.Current.Id, 500, 50, timestamp, new PortfolioInfoItem(new Network("Bitstamp")));

                //M.Send(new PortfolioNetworksMessage(DateTime.UtcNow, new List<PortfolioDistributionItem>()));
            });
        }

        private Brush StringColor(string str)
        {
            var names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            var randomColorName = names[(uint)str.GetHashCode() % names.Length];
            return (SolidColorBrush)new BrushConverter().ConvertFromString(randomColorName.ToString());
        }

        private readonly UserContext _context;

        public void Dispose()
        {
            M.UnregisterAsync(this);
        }
    }
}
