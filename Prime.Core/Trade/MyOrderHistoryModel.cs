using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Trade
{
    public class MyOrderHistoryModel
    {
        public decimal Amount { get; }
        public decimal Size { get; }
        public decimal Total { get; }
        public decimal Sum { get; }

        public MyOrderHistoryModel(decimal amount, decimal size, decimal total, decimal sum)
        {
            this.Amount = amount;
            this.Size = size;
            this.Total = total;
            this.Sum = sum;
        }
    }
}
