using System.Collections.Generic;

namespace Prime.Common
{
    public class TradeOrdersResponse : ResponseModelBase
    {
        public IEnumerable<TradeOrderStatus> Orders { get; set; }

        public TradeOrdersResponse(IEnumerable<TradeOrderStatus> orders)
        {
            Orders = orders;
        }
    }
}