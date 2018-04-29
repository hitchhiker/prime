using System.Collections.Generic;

namespace Prime.Core
{
    public class TransferSuspensions : ResponseModelBase
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