using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Trade
{
    public class OrderBookModel
    {
        public decimal Bid { get; }
        public decimal Size { get; }
        public decimal Total { get; }
        public decimal Sum { get; }
        
        public OrderBookModel(decimal bid, decimal size, decimal total, decimal sum)
        {
            this.Bid = bid;
            this.Size = size;
            this.Total = total;
            this.Sum = sum;
        }
    }
}
