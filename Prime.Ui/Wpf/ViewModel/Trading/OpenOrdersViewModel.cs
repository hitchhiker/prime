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
    public class OpenOrdersViewModel
    {
        private readonly BuySellViewModel _parentModel;
        private readonly Random random = new Random();

        public OpenOrdersViewModel(BuySellViewModel model)
        {
            _parentModel = model;
            new Task(PopulateDataStructures).Start();
        }

        public BindingList<OpenOrdersModel> ListOpenOrders { get; private set; }

        private void PopulateDataStructures()
        {
            ListOpenOrders = new BindingList<OpenOrdersModel>();

            for (int i = 0; i < 5; i++)
            {
                ListOpenOrders.Add(new OpenOrdersModel(DateTime.Now, (i % 2 == 0 ? "Good 'Til Cancelled" : "Immediate or Cancel"), (i % 2 == 0 ? "Bid" : "Ask"), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
            }
        }
    }
}
