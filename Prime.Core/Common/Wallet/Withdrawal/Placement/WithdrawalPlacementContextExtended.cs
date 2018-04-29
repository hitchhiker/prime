using System;

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
