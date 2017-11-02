using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common
{
    public class WithdrawalPlacementContext : NetworkProviderPrivateContext
    {
        public WithdrawalPlacementContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }

        public string Address { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
