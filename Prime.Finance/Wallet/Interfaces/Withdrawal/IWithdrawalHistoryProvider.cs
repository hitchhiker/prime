using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Finance.Wallet.Withdrawal.History;

namespace Prime.Finance
{
    public interface IWithdrawalHistoryProvider
    {
        Task<List<WithdrawalHistoryEntry>> GetWithdrawalHistoryAsync(WithdrawalHistoryContext context);
    }
}