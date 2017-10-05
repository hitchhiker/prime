using Prime.Core;
using Prime.Core.Trade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Ui.Wpf.ViewModel.Trading
{
    public class MarketHistoryViewModel
    {
        private readonly BuySellViewModel _parentModel;
        private readonly Random random = new Random();

        public MarketHistoryViewModel(BuySellViewModel model)
        {
            _parentModel = model;
            new Task(PopulateDataStructures).Start();
        }

        public BindingList<MarketHistoryModel> ListMarketHistory { get; private set; }

        private void PopulateDataStructures()
        {
            ListMarketHistory = new BindingList<MarketHistoryModel>();

            for (int i = 0; i < 5; i++)
            {
                ListMarketHistory.Add(new MarketHistoryModel(DateTime.Now, new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), (i % 2 == 0 ? "Buy" : "Sell"), (i % 2 == 0 ? "Bid" : "Ask")));
            }
        }
    }
}
