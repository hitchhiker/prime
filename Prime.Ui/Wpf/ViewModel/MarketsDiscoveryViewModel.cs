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
            random = new Random();
            new Task(PopulateGrid).Start();
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;
        private Random random = null;

        public BindingList<MarketControlViewModel> ListMarketControls { get; private set; }

        private void PopulateGrid()
        {
            ListMarketControls = new BindingList<MarketControlViewModel>() {
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (2 + 2) - 2), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/BTC.png", "BTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (2 + 2) - 2), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/DASH.png", "DASH")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (2 + 2) - 2), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETC.png", "ETC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (2 + 2) - 2), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/ETH.png", "ETHEREUM")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (2 + 2) - 2), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/LTC.png", "LTC")),
                new MarketControlViewModel(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (2 + 2) - 2), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), "../../Asset/img/XRP.png", "XRP"))
            };
        }

        public override CommandContent Create()
        {
            return new SimpleContentCommand("markets discovery");
        }
    }
}
