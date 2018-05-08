using Prime.Common;
using Prime.Common.Misc;
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
    public class CoinsViewModel : DocumentPaneViewModel
    {
        public CoinsViewModel(ScreenViewModel screenViewModel)
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            random = new Random();
            new Task(PopulateGrid).Start();
        }

        private Random random = null;
        public BindingList<CoinsItemModel> ListCoinItems { get; private set; }

        private void PopulateGrid()
        {
            ListCoinItems = new BindingList<CoinsItemModel>();

            for (int i = 0; i < 5; i++)
            {
                ListCoinItems.Add(new CoinsItemModel((decimal)(random.NextDouble() * (1 + 1) - 1), new Money((decimal)(random.NextDouble() * (1 - 0) + 0)), new Money((decimal)(random.NextDouble() * (1000 - 100) + 100)), (i % 2 == 0 ? "../../Asset/img/ETH.png" : "../../Asset/img/BTC.png"), (i % 2 == 0 ? "Etherium" : "Bitcoin"), (i % 2 == 0 ? "ETH" : "BTC"), (i % 2 == 0 ? "Ethash" : "SHA256"), "PoW", (i % 2 == 0 ? "6.88 M" : "2.04 M"), "Tidex", DateTime.Now.AddMinutes(-random.NextDouble() * (1000 - 0) + 0)));
            }
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("coins");
        }
    }
}
