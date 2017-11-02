using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common.Wallet.Withdrawal.Confirmation
{
    public class WithdrawalConfirmationContext : NetworkProviderContext
    {
        public WithdrawalConfirmationContext(string withdrawalId, ILogger logger = null) : base(logger)
        {
            WithdrawalId = withdrawalId;
        }

        public string WithdrawalId { get; set; }
    }
}
