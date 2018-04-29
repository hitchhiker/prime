using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Trade
{
    public class OrderBookModel
    {
        public Money Bid { get; }
        public Money Size { get; }
        public Money Total { get; }
        public Money Sum { get; }
        
        public OrderBookModel(Money bid, Money size, Money total, Money sum)
        {
            this.Bid = bid;
            this.Size = size;
            this.Total = total;
            this.Sum = sum;
        }
    }
}
