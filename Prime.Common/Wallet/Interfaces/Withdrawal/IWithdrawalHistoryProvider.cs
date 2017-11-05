using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Common.Wallet.Withdrawal.History;

namespace Prime.Common
{
    public interface IWithdrawalHistoryProvider
    {
        Task<List<WithdrawalHistoryEntry>> GetWithdrawalHistory(WithdrawalHistoryContext context);
    }
}