using Prime.Core;
using Prime.Core.Trade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Ui.Wpf.ViewModel
{
    public class MyOrderHistoryViewModel
    {
        private readonly BuySellViewModel _parentModel;
        private readonly Random random = new Random();

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
                ListMyOrderHistory.Add(new MyOrderHistoryModel((i % 2 == 0 ? "Good 'Til Cancelled" : "Immediate or Cancel"), (i % 2 == 0 ? "Bid" : "Ask"), DateTime.Now, DateTime.Now, new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
            }
        }
    }
}
