using System;
using Prime.Core;
using Prime.Core.Exchange.Rates;
using Prime.Core.Exchange.Model;

namespace Prime.Ui.Wpf.ViewModel
{
    public class MarketControlViewModel
    {
        public decimal ChangePerc { get; }
        public string IconPath { get; }
        public string Name { get; }
        public Money Buy { get ; }
        public Money Sell { get; }

        public MarketControlViewModel()
        {
        }

        public MarketControlViewModel(MarketsDiscoveryItemModel model)
        {
            ChangePerc = model.ChangePerc;
            IconPath = model.IconPath;
            Name = model.Name;
            Buy = model.Buy;
            Sell = model.Sell;
        }
    }
}
