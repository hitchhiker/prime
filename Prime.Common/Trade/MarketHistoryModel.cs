using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Trade
{
    public class MarketHistoryModel
    {
        public string BuySell { get; }
        public string BidAsk { get; }
        public DateTime Date { get; }
        public Money TotalUnits { get; }
        public Money TotalCost { get; }

        public MarketHistoryModel(DateTime date, Money totalUnits, Money totalCost, string buySell, string bidAsk)
        {
            this.Date = date;
            this.TotalUnits = totalUnits;
            this.TotalCost = totalCost;
            this.BuySell = buySell;
            this.BidAsk = bidAsk;
        }
    }
}
