using System;
using System.Collections.Generic;
using System.Text;

namespace Prime.Common
{
    public class WithdrawalHistoryEntry
    {
        public string WithdrawalRemoteId { get; set; }
        public Asset Asset { get; set; }
        public string Address { get; set; }
        public decimal Fee { get; set; }

        public DateTime CreatedTimeUtc { get; set; }
    }
}
