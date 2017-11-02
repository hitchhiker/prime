using System;
using System.Collections.Generic;
using System.Text;
using Prime.Utility;

namespace Prime.Common.Wallet.Withdrawal.Confirmation
{
    public class WithdrawalConfirmationContext : NetworkProviderContext
    {
        public WithdrawalConfirmationContext(string transactionId, ILogger logger = null) : base(logger)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; set; }
    }
}
