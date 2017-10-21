using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Common.Trade
{
    public class TradeTypeModel
    {
        public int Key { get; }
        public string TradeTypeName { get; }

        public TradeTypeModel(int key, string tradeTypeName)
        {
            this.Key = key;
            this.TradeTypeName = tradeTypeName;
        }
    }
}
