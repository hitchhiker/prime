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
            CustomFee = null;
        }

        public WalletAddress Address { get; set; }

        public Money Amount { get; set; }


        /// <summary>
        /// Provides ability to set custom withdrawal fee if provider supports it.
        /// </summary>
        public Money? CustomFee { get; set; }

        /// <summary>
        /// Used to provide any authentication tokens (e.g. 2FA token).
        /// </summary>
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// This field can store paymentId or any other relevant information.
        /// </summary>
        public string Description { get; set; }

        public bool HasDescription => !String.IsNullOrWhiteSpace(Description);
        public bool HasAuthToken => !String.IsNullOrWhiteSpace(AuthenticationToken);
        public bool HasCustomFee => CustomFee.HasValue;
    }
}
