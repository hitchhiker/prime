using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Core.Trade;

namespace Prime.Ui.Wpf.ViewModel.Trading
{
    public class MyOrderHistoryViewModel
    {
        private readonly BuySellViewModel _parentModel;
        private readonly Random _random = new Random();

        public MyOrderHistoryViewModel(BuySellViewModel model)
        {
            _parentModel = model;
            new Task(PopulateDataStructures).Start();
        }

        public BindingList<MyOrderHistoryModel> ListMyOrderHistory { get; private set; }

        private void PopulateDataStructures()
        {
            ListMyOrderHistory = new BindingList<MyOrderHistoryModel>();

            for (int i = 0; i < 5; i++)
            {
                ListMyOrderHistory.Add(new MyOrderHistoryModel((i % 2 == 0 ? "Good 'Til Cancelled" : "Immediate or Cancel"), (i % 2 == 0 ? "Bid" : "Ask"), DateTime.Now, DateTime.Now, new Money((decimal)(_random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(_random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(_random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(_random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
            }
        }
    }
}
