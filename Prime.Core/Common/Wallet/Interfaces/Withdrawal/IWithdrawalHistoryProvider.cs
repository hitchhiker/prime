using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Core.Wallet.Withdrawal.History;

namespace Prime.Core
{
    public interface IWithdrawalHistoryProvider
    {
        Task<List<WithdrawalHistoryEntry>> GetWithdrawalHistoryAsync(WithdrawalHistoryContext context);
    }
}