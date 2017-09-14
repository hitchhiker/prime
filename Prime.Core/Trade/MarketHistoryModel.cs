using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Trade
{
    public class MarketHistoryModel
    {
        public decimal TotalUnits { get; }
        public decimal TotalCost { get; }
        public string BuySell { get; }
        public string BidAsk { get; }
        public DateTime Date { get; }

        public MarketHistoryModel(DateTime date, decimal totalUnits, decimal totalCost, string buySell, string bidAsk)
        {
            this.Date = date;
            this.TotalUnits = totalUnits;
            this.TotalCost = totalCost;
            this.BuySell = buySell;
            this.BidAsk = bidAsk;
        }
    }
}
