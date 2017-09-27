using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prime.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Prime.Ui.Wpf.View.Markets
{
    /// <summary>
    /// Interaction logic for MarketControl.xaml
    /// </summary>
    public partial class MarketControl : UserControl, INotifyPropertyChanged
    {
        public MarketControl()
        {
            InitializeComponent();
            InitialiseChart();
            LayoutRoot.DataContext = this; //Very important
        }

        private double _lastBuySell;
        private double _trend;
        public SeriesCollection BuySellSeries { get; set; }

        public static readonly DependencyProperty sellTextProperty = DependencyProperty.Register("Sell", typeof(Money), typeof(MarketControl), new FrameworkPropertyMetadata(Money.Zero));
        public static readonly DependencyProperty buyTextProperty = DependencyProperty.Register("Buy", typeof(Money), typeof(MarketControl), new FrameworkPropertyMetadata(Money.Zero));
        public static readonly DependencyProperty marketNameTextProperty = DependencyProperty.Register("MarketName", typeof(string), typeof(MarketControl), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty changePercTextProperty = DependencyProperty.Register("ChangePerc", typeof(decimal), typeof(MarketControl), new FrameworkPropertyMetadata(decimal.Zero));
        public static readonly DependencyProperty iconPathTextProperty = DependencyProperty.Register("IconPath", typeof(string), typeof(MarketControl), new FrameworkPropertyMetadata(string.Empty));

        public Money Sell
        {
            get { return (Money)GetValue(sellTextProperty); }
            set { SetValue(sellTextProperty, value); }
        }

        public Money Buy
        {
            get { return (Money)GetValue(buyTextProperty); }
            set { SetValue(buyTextProperty, value); }
        }

        public decimal ChangePerc
        {
            get { return (decimal)GetValue(changePercTextProperty); }
            set { SetValue(changePercTextProperty, value); }
        }

        public string MarketName
        {
            get { return GetValue(marketNameTextProperty).ToString(); }
            set { SetValue(marketNameTextProperty, value); }
        }

        public string IconPath
        {
            get { return GetValue(iconPathTextProperty).ToString(); }
            set { SetValue(iconPathTextProperty, value); }
        }

        public double LastBuySell
        {
            get { return _lastBuySell; }
            set
            {
                _lastBuySell = value;
                OnPropertyChanged("LastBuySell");
            }
        }

        private void SetBuySell()
        {
            var target = ((ChartValues<ObservableValue>)BuySellSeries[0].Values).Last().Value;
            var step = (target - _lastBuySell) / 4;

            Task.Run(() =>
            {
                for (var i = 0; i < 4; i++)
                {
                    Thread.Sleep(100);
                    LastBuySell += step;
                }
                LastBuySell = target;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitialiseChart()
        {
            BuySellSeries = new SeriesCollection
            {
                new LineSeries
                {
                    AreaLimit = -10,
                    Values = new ChartValues<ObservableValue>
                    {
                        new ObservableValue(3),
                        new ObservableValue(5),
                        new ObservableValue(6),
                        new ObservableValue(7),
                        new ObservableValue(3),
                        new ObservableValue(4),
                        new ObservableValue(2),
                        new ObservableValue(5),
                        new ObservableValue(8),
                        new ObservableValue(3),
                        new ObservableValue(5),
                        new ObservableValue(6),
                        new ObservableValue(7),
                        new ObservableValue(3),
                        new ObservableValue(4),
                        new ObservableValue(2),
                        new ObservableValue(5),
                        new ObservableValue(8)
                    }
                }
            };
            _trend = 8;


            Task.Run(() =>
            {
                var r = new Random();
                while (true)
                {
                    Thread.Sleep(500);
                    _trend += (r.NextDouble() > 0.3 ? 1 : -1) * r.Next(0, 5);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        BuySellSeries[0].Values.Add(new ObservableValue(_trend));
                        BuySellSeries[0].Values.RemoveAt(0);
                        SetBuySell();
                    });
                }
            });
        }
    }
}
