using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common
{
    [Obsolete]
    public class WithdrawalPlacementContextExtended : WithdrawalPlacementContext
    {
        public WithdrawalPlacementContextExtended(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}
