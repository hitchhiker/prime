using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common.Wallet.Withdrawal.History
{
    public class WithdrawalHistoryContext : NetworkProviderPrivateContext
    {
        public WithdrawalHistoryContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }

        public Asset Asset { get; set; }

        /// <summary>
        /// Optional. Some providers can use it.
        /// </summary>
        public DateTime FromTimeUtc { get; set; }
    }
}
