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
using Prime.Utility;

namespace Prime.Ui.Wpf.ViewModel
{
    public class PortfolioPieChartViewModel : VmBase, IDisposable
    {
        public PortfolioPieChartViewModel()
        {
            _context = UserContext.Current;
            new Task(PopulatePieChart).Start();
            M.RegisterAsync<PortfolioChangedMessage>(this, PortfolioChangedMessage);
        }

        private void PortfolioChangedMessage(PortfolioChangedMessage m)
        {
            UiDispatcher.Invoke(() =>
                {
                }
            );
        }

        public ObservableCollection<PortfolioInfoItem> ListPieChartItems { get; private set; }

        private void PopulatePieChart()
        {
            UiDispatcher.Invoke(() =>
            {
                ListPieChartItems = new ObservableCollection<PortfolioInfoItem>
                {
                    new PortfolioInfoItem(new Network("Poloniex")),
                    new PortfolioInfoItem(new Network("Bitstamp")),
                    new PortfolioInfoItem(new Network("Bittrex"))
                };
            });

            //Send message to core that new exchanges and percentages have been added
        }

        private readonly UserContext _context;

        public void Dispose()
        {
            M.UnregisterAsync(this);
        }
    }
}
