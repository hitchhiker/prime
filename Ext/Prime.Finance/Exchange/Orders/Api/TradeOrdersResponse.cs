﻿using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance
{
    /// <summary>
    /// TODO: AY: this class has same members as OpenOrdersResponse.
    /// </summary>
    public class TradeOrdersResponse : ResponseModelBase
    {
        public IEnumerable<TradeOrderStatus> Orders { get; set; }

        public TradeOrdersResponse(IEnumerable<TradeOrderStatus> orders)
        {
            Orders = orders;
        }
    }
}