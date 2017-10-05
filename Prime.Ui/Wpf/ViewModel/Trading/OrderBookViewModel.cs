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
    public class OrderBookViewModel
    {
        private readonly BuySellViewModel _parentModel;
        private readonly Random random = new Random();

        public OrderBookViewModel(BuySellViewModel model)
        {
            _parentModel = model;
            new Task(PopulateDataStructures).Start();
        }

        public BindingList<OrderBookModel> ListOrderBookBuy { get; private set; }
        public BindingList<OrderBookModel> ListOrderBookSell { get; private set; }

        private void PopulateDataStructures()
        {
            ListOrderBookBuy = new BindingList<OrderBookModel>();
            ListOrderBookSell = new BindingList<OrderBookModel>();

            for (int i = 0; i < 5; i++)
            {
                ListOrderBookBuy.Add(new OrderBookModel(new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10 - 0.001) + 0.001), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
                ListOrderBookSell.Add(new OrderBookModel(new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10 - 0.001) + 0.001), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw()), new Money((decimal)(random.NextDouble() * (10000 - 1000) + 1000), "BTC".ToAssetRaw())));
            }
        }
    }
}
