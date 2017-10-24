using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Prime.Common;
using Prime.Common.Portfolio;
using System.Windows;
using Prime.Common.Wallet;

namespace Prime.Ui.Wpf.ViewModel
{
    public class PortfolioPieChartViewModel : DocumentPaneViewModel
    {
        public PortfolioPieChartViewModel()
        {
            _context = UserContext.Current;
            Dispatcher = Application.Current.Dispatcher;
            new Task(PopulatePieChart).Start();
        }

        public ObservableCollection<PortfolioInfoItem> ListPieChartItems { get; private set; }

        private void PopulatePieChart()
        {
            ListPieChartItems = new ObservableCollection<PortfolioInfoItem>
            {
                new PortfolioInfoItem(new Network("Poloniex")),
                new PortfolioInfoItem(new Network("Bitstamp")),
                new PortfolioInfoItem(new Network("Bittrex"))
            };

            //Send message to core that new exchanges and percentages have been added
        }

        public readonly Dispatcher Dispatcher;
        private readonly UserContext _context;

        public override CommandContent GetPageCommand()
        {
            return new SimpleContentCommand("portfolio pie chart");
        }
    }
}
