using System.Collections.Generic;
using Prime.Core;

namespace Prime.Finance
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