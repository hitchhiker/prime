using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core.Trade
{
    public class OpenOrdersModel
    {
        public decimal Amount { get; }
        public decimal Size { get; }
        public decimal Total { get; }
        public string Status { get; }

        public OpenOrdersModel(decimal amount, decimal size, decimal total, string status)
        {
            this.Amount = amount;
            this.Size = size;
            this.Total = total;
            this.Status = status;
        }
    }
}
