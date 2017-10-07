using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prime.Core;
using Prime.Core.Exchange.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class MarketsDiscoveryViewModel : DocumentPaneViewModel, INotifyPropertyChanged
    {
        public MarketsDiscoveryViewModel(ScreenViewModel screenViewModel)
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            _random = new Random();
            new Task(PopulateGrid).Start();
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;
        private readonly Random _random = null;

        public BindingList<MarketControlViewModel> ListMarketControls { get; private set; }

        private void PopulateGrid()
        {
            ListMarketControls = new BindingList<MarketControlViewModel>() {
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/BTC.png", "BTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/DASH.png", "DASH")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETC.png", "ETC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETH.png", "ETHEREUM")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/LTC.png", "LTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/XRP.png", "XRP")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/BTC.png", "BTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/DASH.png", "DASH")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETC.png", "ETC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETH.png", "ETHEREUM")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/LTC.png", "LTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/XRP.png", "XRP")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/BTC.png", "BTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/DASH.png", "DASH")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETC.png", "ETC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETH.png", "ETHEREUM")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/LTC.png", "LTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/XRP.png", "XRP")) };
        }

        public void LoadManyControls(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                ListMarketControls.Add(new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(_random.NextDouble() * (2 + 2) - 2), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(_random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/BTC.png", "BTC")));
            }
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("markets discovery");
        }
    }
}
