using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Finance.Wallet.Withdrawal.Confirmation
{
    public class WithdrawalConfirmationContext : NetworkProviderPrivateContext
    {
        public WithdrawalConfirmationContext(UserContext userContext, ILogger logger = null) : base(userContext, logger)
        {
        }

        public string WithdrawalRemoteId { get; set; }
    }
}
