using Prime.Common;
using Prime.Common.Portfolio;
using Prime.Common.Wallet;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Prime.Ui.Wpf.ViewModel
{
    public class PortfolioViewModel : DocumentPaneViewModel
    {
        public PortfolioViewModel() { }

        public PortfolioViewModel(ScreenViewModel screenViewModel)
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            random = new Random();
            new Task(PopulateGrid).Start();
        }

        private Random random = null;
        public BindingList<PortfolioItemModel> ListPortfolioItems { get; private set; }

        private void PopulateGrid()
        {
            ListPortfolioItems = new BindingList<PortfolioItemModel>();

            for (int i = 0; i < 5; i++)
            {
                ListPortfolioItems.Add(new PortfolioItemModel((decimal)(random.NextDouble() * (1000 - 100) + 100), (decimal)(random.NextDouble() * (1000 - 100) + 100), (decimal)(random.NextDouble() * (1 + 1) - 1), (decimal)(random.NextDouble() * (1000 - 100) + 100), (decimal)(random.NextDouble() * (1000 - 100) + 100), (i % 2 == 0 ? "../../Asset/img/CAT.png" : "../../Asset/img/Apple.png"), (i % 2 == 0 ? "CAT" : "AAPL"), (i % 2 == 0 ? "" : "Apple"), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000)), new Money((decimal)(random.NextDouble() * (10000 + 10000) - 10000)), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000))));
            }
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("portfolio");
        }
    }
}
