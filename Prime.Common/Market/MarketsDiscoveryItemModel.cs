using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Exchange.Model
{
    public class MarketsDiscoveryItemModel
    {
        public decimal ChangePerc { get; }
        public string IconPath { get; }
        public string Name { get; }
        public Money Buy { get; }
        public Money Sell { get; }

        public MarketsDiscoveryItemModel(decimal changePerc, Money buy, Money sell, string iconPath, string name)
        {
            this.ChangePerc = changePerc;
            this.Buy = buy;
            this.Sell = sell;
            this.IconPath = iconPath;
            this.Name = name;
        }
    }
}
