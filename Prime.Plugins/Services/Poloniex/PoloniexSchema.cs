using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Plugins.Services.Poloniex
{
    internal class PoloniexSchema
    {
        internal class BalancesDetailedResponse : Dictionary<string, BalanceDetailedResponse>
        {
            
        }

        internal class BalanceDetailedResponse
        {
            public decimal available;
            public decimal onOrders;
            public decimal btcValue;
        }
    }
}
