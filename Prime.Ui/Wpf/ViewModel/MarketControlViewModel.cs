using System;
using Prime.Core;
using Prime.Core.Exchange.Rates;
using Prime.Core.Exchange.Model;

namespace Prime.Ui.Wpf.ViewModel
{
    public class MarketControlViewModel
    {
        private decimal _changePerc = 0;
        private string _iconPath = "";
        private string _name = "";
        private Money _buy;
        private Money _sell;

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

        public decimal ChangePerc { get => _changePerc; private set => _changePerc = value; }
        public string IconPath { get => _iconPath; private set => _iconPath = value; }
        public string Name { get => _name; private set => _name = value; }
        public Money Buy { get => _buy; private set => _buy = value; }
        public Money Sell { get => _sell; private set => _sell = value; }
    }
}
