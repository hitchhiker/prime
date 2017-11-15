using System.Collections.Generic;

namespace Prime.Common
{
    public class TransferSuspensions
    {
        public readonly IReadOnlyList<Asset> DepositSuspended;
        public readonly IReadOnlyList<Asset> WithdrawalSuspended;

        public TransferSuspensions(IReadOnlyList<Asset> depositSuspended, IReadOnlyList<Asset> withdrawalSuspended)
        {
            DepositSuspended = depositSuspended;
            WithdrawalSuspended = withdrawalSuspended;
        }
    }
}