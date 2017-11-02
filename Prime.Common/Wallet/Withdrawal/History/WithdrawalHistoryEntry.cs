using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common.Wallet.Withdrawal.History
{
    public class WithdrawalHistoryEntry
    {
        public string WithdrawalId { get; set; }
        public string Currency { get; set; }
        public string Address { get; set; }
        public decimal Fee { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
