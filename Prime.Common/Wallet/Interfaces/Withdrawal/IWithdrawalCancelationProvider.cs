using System.Threading.Tasks;
using Prime.Common.Wallet.Withdrawal;
using Prime.Common.Wallet.Withdrawal.Cancelation;

namespace Prime.Common
{
    public interface IWithdrawalCancelationProvider
    {
        Task<WithdrawalCancelationResult> CancelWithdrawalAsync(WithdrawalCancelationContext context);
    }
}