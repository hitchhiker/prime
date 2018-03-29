using System.Collections.Generic;

namespace Prime.Common
{
    public class OpenOrdersResponse : ResponseModelBase
    {
        public IEnumerable<TradeOrderStatus> Orders { get; }

        public OpenOrdersResponse(IEnumerable<TradeOrderStatus> orders)
        {
            Orders = orders;
        }
    }
}