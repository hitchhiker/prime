using System.Collections.Generic;

namespace Prime.Core
{
    /// <summary>
    /// TODO: AY: this class has same members as TradeOrdersResponse.
    /// </summary>
    public class OpenOrdersResponse : ResponseModelBase
    {
        public IEnumerable<TradeOrderStatus> Orders { get; }

        public OpenOrdersResponse(IEnumerable<TradeOrderStatus> orders)
        {
            Orders = orders;
        }
    }
}