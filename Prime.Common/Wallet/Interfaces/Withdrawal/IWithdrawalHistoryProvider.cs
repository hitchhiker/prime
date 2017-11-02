using System.Threading.Tasks;
using Prime.Common.Wallet.Withdrawal.History;

namespace Prime.Common
{
    public interface IWithdrawalHistoryProvider
    {
        Task<WithdrawalHistoryEntry> GetWithdrawalHistory(WithdrawalHistoryContext context);
    }
}