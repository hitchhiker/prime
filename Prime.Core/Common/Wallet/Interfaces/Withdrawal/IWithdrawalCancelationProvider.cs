using System.Threading.Tasks;
using Prime.Core.Wallet.Withdrawal;
using Prime.Core.Wallet.Withdrawal.Cancelation;

namespace Prime.Core
{
    public interface IWithdrawalCancelationProvider
    {
        Task<WithdrawalCancelationResult> CancelWithdrawalAsync(WithdrawalCancelationContext context);
    }
}