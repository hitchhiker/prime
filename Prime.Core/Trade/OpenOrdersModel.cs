using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Trade
{
    public class OpenOrdersModel
    {
        public DateTime Date { get; }
        public string Type { get; }
        public string BidAsk { get; }
        public Money UnitsFilled { get; }
        public Money UnitsTotal { get; }
        public Money ActualRate { get; }
        public Money EstimatedTotal { get; }

        public OpenOrdersModel(DateTime date, string type, string bidAsk, Money unitsFilled, Money unitsTotal, Money actualRate, Money estimatedTotal)
        {
            this.Date = date;
            this.Type = type;
            this.BidAsk = bidAsk;
            this.UnitsFilled = unitsFilled;
            this.UnitsTotal = unitsTotal;
            this.ActualRate = actualRate;
            this.EstimatedTotal = estimatedTotal;
        }
    }
}
