using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common.Wallet.Withdrawal.History
{
    public class WithdrawalHistoryContext : NetworkProviderContext
    {
        public Asset Asset { get; set; }

        /// <summary>
        /// Optional. Some providers can use it.
        /// </summary>
        public DateTime FromTimeUtc { get; set; }
    }
}
