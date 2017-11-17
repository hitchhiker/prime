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

        public WalletAddress Address { get; set; }

        public Money Amount { get; set; }
    }
}
