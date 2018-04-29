using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Core.Wallet.Withdrawal.Cancelation
{
    public class WithdrawalCancelationContext : NetworkProviderContext
    {
        public string WithdrawalRemoteId { get; set; }
    }
}
