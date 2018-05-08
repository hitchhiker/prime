using System;
using Prime.Core;

namespace Prime.Finance
{
    [Obsolete]
    public class WithdrawalPlacementContextExtended : WithdrawalPlacementContext
    {
        public WithdrawalPlacementContextExtended(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }
    }
}
