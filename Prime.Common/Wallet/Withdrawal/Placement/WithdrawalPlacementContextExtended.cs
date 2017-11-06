using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common
{
    public class WithdrawalPlacementContextExtended : WithdrawalPlacementContext
    {
        public WithdrawalPlacementContextExtended(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }

        /// <summary>
        /// Provides ability to set custom withdrawal fee if provider supports it.
        /// </summary>
        public Money CustomFee { get; set; }

        /// <summary>
        /// Used to provide any authentication tokens (e.g. 2FA token).
        /// </summary>
        public string AuthenticationToken { get; set; } 

        /// <summary>
        /// This field can store paymentId or any other relevant information.
        /// </summary>
        public string Description { get; set; }
    }
}
