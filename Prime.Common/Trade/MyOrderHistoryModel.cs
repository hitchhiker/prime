using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Trade
{
    public class MyOrderHistoryModel
    {
        public string Type { get; }
        public string BidAsk { get; }
        public DateTime ClosedDate { get; }
        public DateTime OpenedDate { get; }
        public Money UnitsFilled { get; }
        public Money UnitsTotal { get; }
        public Money ActualRate { get; }
        public Money CostProceeds { get; }

        public MyOrderHistoryModel(string type, string bidAsk, DateTime closedDate, DateTime openedDate, Money unitsFilled, Money unitsTotal, Money actualRate, Money costProceeds)
        {
            this.Type = type;
            this.BidAsk = bidAsk;
            this.ClosedDate = closedDate;
            this.OpenedDate = openedDate;
            this.UnitsFilled = unitsFilled;
            this.UnitsTotal = unitsTotal;
            this.ActualRate = actualRate;
            this.CostProceeds = costProceeds;
        }
    }
}
