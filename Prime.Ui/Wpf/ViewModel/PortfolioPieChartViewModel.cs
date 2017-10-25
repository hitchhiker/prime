using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Prime.Common;
using Prime.Common.Portfolio;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using prime;
using Prime.Common.Wallet;
using Prime.Common.Wallet.Portfolio.Messages;
using Prime.Utility;
using Xceed.Wpf.Toolkit;
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
            M.RegisterAsync<ExchangePortfolioChangedMessage>(this, ExchangePortfolioChangedMessage);
        }

        private void ExchangePortfolioChangedMessage(ExchangePortfolioChangedMessage m)
        {
            UiDispatcher.Invoke(() =>
                {
                    UpdatePieChart(m.PortfolioInfoItem.Network, m.Percentage, m.Amount);
                }
            );
        }

        private void UpdatePieChart(Network network, decimal percentage, decimal amount)
        {
            UiDispatcher.Invoke(() =>
                {
                    lock (_lock)
                    {
                        string name = network.Name;

                        //var lp = _lastPercentages.GetOrAdd(network, percentage);

                        //if (Math.Abs(lp - percentage) < 1)
                        //    continue;

                        _lastPercentages.AddOrUpdate(network, percentage, (_, __) => percentage);

                        var e = SeriesPieChartItems.FirstOrDefault(x => x.Title == name);

                        PieSeries seriesItem = new PieSeries
                        {
                            Title = network.Name,
                            Fill = StringColor(name),
                            Values = new ChartValues<double>() { (double)amount },
                            DataLabels = true,
                            LabelPosition = PieLabelPosition.InsideSlice,
                            Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#555"),
                            LabelPoint = c => c.Participation > 0.05 ? $"{Math.Round(c.Participation, 2):P}" : null
                        };

                        if (e != null)
                            e.Values = new ChartValues<double>() { (double)amount };
                        else
                            SeriesPieChartItems.Add(seriesItem);
                    }
                }
            );
        }

        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<Network, decimal> _lastPercentages = new ConcurrentDictionary<Network, decimal>();

        public SeriesCollection SeriesPieChartItems { get; }

        private void PopulateTestData()
        {
            UiDispatcher.Invoke(() =>
            {
                DateTime timestamp = DateTime.UtcNow;

                ExchangePortfolioChangedMessage message1 = new ExchangePortfolioChangedMessage(UserContext.Current.Id, 200, 20, timestamp, new PortfolioInfoItem(new Network("Poloniex")));
                ExchangePortfolioChangedMessage message2 = new ExchangePortfolioChangedMessage(UserContext.Current.Id, 300, 30, timestamp, new PortfolioInfoItem(new Network("Bittrex")));
                ExchangePortfolioChangedMessage message3 = new ExchangePortfolioChangedMessage(UserContext.Current.Id, 500, 50, timestamp, new PortfolioInfoItem(new Network("Bitstamp")));

                M.Send(message1, UserContext.Current.Token);
                M.Send(message2, UserContext.Current.Token);
                M.Send(message3, UserContext.Current.Token);
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
