using System;
using System.Collections.Generic;
using System.Text;
using Prime.Core;

namespace Prime.Finance.Wallet.Withdrawal.Cancelation
{
    public class WithdrawalCancelationContext : NetworkProviderContext
    {
        public string WithdrawalRemoteId { get; set; }
    }
}
