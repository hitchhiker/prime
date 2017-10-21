using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Watchlist
{
    public class WatchlistItemModel
    {
        public decimal ChangePerc { get; }
        public string IconPath { get; }
        public string Market { get; }
        public string MarketDescription { get; }
        public Money Sell { get; }
        public Money Buy { get; }

        public WatchlistItemModel(decimal changePerc, Money sell, Money buy, string iconPath, string market, string marketDescription)
        {
            this.ChangePerc = changePerc;
            this.Sell = sell;
            this.Buy = buy;
            this.IconPath = iconPath;
            this.Market = market;
            this.MarketDescription = marketDescription;
        }
    }
}
