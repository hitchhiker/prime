using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common.Wallet.Withdrawal.Confirmation
{
    public class WithdrawalConfirmationContext : NetworkProviderContext
    {
        public WithdrawalConfirmationContext(string withdrawalRemoteId, ILogger logger = null) : base(logger)
        {
            WithdrawalRemoteId = withdrawalRemoteId;
        }

        public string WithdrawalRemoteId { get; set; }
    }
}
