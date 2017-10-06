using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prime.Core;
using Prime.Ui.Wpf.View.Trade;
using Prime.Ui.Wpf.ViewModel.Trading;
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
            Chart.DataContext = this;
        }

        private double _lastBuySell;
        private double _trend;
        public SeriesCollection BuySellSeries { get; set; }

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


            /*Task.Run(() =>
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
            });*/
        }

        private async void LoadModalAsync(UserControl content, string dialogName, string contentName)
        {
            await Dispatcher.Invoke(async () =>
            {
                var mw = this.TryFindParent<MahApps.Metro.Controls.MetroWindow>();
                if (mw == null)
                    return;

                var dialog = Application.Current.Resources[dialogName] as BaseMetroDialog;
                if (dialog == null)
                    return;

                dialog.Height = 150;

                await mw.ShowMetroDialogAsync(dialog,
                    new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted, AnimateShow = false, AnimateHide = false });

                var c = dialog.FindChild<Canvas>(contentName);
                c.Children.Clear();
                c.Children.Add(content);

                void DetectOutClick(object o, MouseButtonEventArgs args)
                {
                    var hit = VisualTreeHelper.HitTest(dialog, Mouse.GetPosition(dialog)) != null;
                    if (hit)
                        return;
                    
                    mw.HideMetroDialogAsync(dialog, new MetroDialogSettings { AnimateShow = false, AnimateHide = false }).ContinueWith(x => { mw.PreviewMouseDown -= DetectOutClick; });
                    args.Handled = true;
                }

                mw.PreviewMouseDown += DetectOutClick;
            });
        }

        private void BtnBuy_OnClick(object sender, RoutedEventArgs e)
        {
            LoadModalAsync(new BuyControl() { DataContext = new BuyViewModel() }, "BuyDialog", "BuyContent");
        }

        private void BtnSell_OnClick(object sender, RoutedEventArgs e)
        {
            LoadModalAsync(new SellControl() { DataContext = new SellViewModel() }, "SellDialog", "SellContent");
        }
    }
}
