using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Exchange
{
    public class BidAskData
    {
        public BidAskData()
        {
            Time = DateTime.Now;
        }

        public BidAskData(Money price, decimal volume)
        {
            Price = price;
            Volume = volume;
        }

        public BidAskData(Money price, decimal volume, DateTime time): this(price, volume)
        {
            Time = time;
        }

        public Money Price { get; set; }
        public decimal Volume { get; set; }
        public DateTime Time { get; set; }
    }
}
