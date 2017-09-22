using Prime.Core;
using Prime.Core.Exchange.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class MarketsDiscoveryViewModel : DocumentPaneViewModel
    {
        public MarketsDiscoveryViewModel(ScreenViewModel screenViewModel)
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            random = new Random();
            new Task(PopulateGrid).Start();
        }

        private Random random = null;
        public BindingList<MarketsDiscoveryItemModel> ListMarketsDiscoveryItems { get; private set; }

        private void PopulateGrid()
        {
            ListMarketsDiscoveryItems = new BindingList<MarketsDiscoveryItemModel>();

            for (int i = 0; i < 5; i++)
            {
                //ListExchangeItems.Add(new MarketsDiscoveryItemModel((decimal)(random.NextDouble() * (1 + 1) - 1), new Money((decimal)(random.NextDouble() * (10000 - 100) + 100)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), (i % 2 == 0 ? "../../Asset/img/CAT.png" : "../../Asset/img/Apple.png"), (i % 2 == 0 ? "CAT" : "AAPL"), (i % 2 == 0 ? "" : "Apple")));
            }
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

        public override CommandContent Create()
        {
            return new SimpleContentCommand("markets discovery");
        }
    }
}
