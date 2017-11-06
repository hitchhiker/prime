using System;

namespace Prime.Common
{
    public class WithdrawalHistoryEntry
    {
        public string WithdrawalRemoteId { get; set; }
        public Money Price { get; set; }
        public string Address { get; set; }
        public Money Fee { get; set; }

        public WithdrawalStatus WithdrawalStatus { get; set; }

        public DateTime CreatedTimeUtc { get; set; }
    }
}
