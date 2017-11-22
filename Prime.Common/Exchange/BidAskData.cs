using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Exchange
{
    public abstract class BidAskData
    {
        protected BidAskData() { }

        protected BidAskData(Money price, decimal volume)
        {
            Price = price;
            Volume = volume;
            UtcUpdated = DateTime.UtcNow;
        }

        public Money Price { get; private set; }
        public decimal Volume { get; private set; }
        public DateTime UtcUpdated { get; private set; }

        public override string ToString()
        {
            return $"Price: {Price.Display} Volume: {Volume} Time: {UtcUpdated}";
        }
    }
}
