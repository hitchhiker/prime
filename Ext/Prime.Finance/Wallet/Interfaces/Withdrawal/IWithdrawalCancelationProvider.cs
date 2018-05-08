using System.Threading.Tasks;
using Prime.Finance.Wallet.Withdrawal.Cancelation;

namespace Prime.Finance
{
    public interface IWithdrawalCancelationProvider
    {
        Task<WithdrawalCancelationResult> CancelWithdrawalAsync(WithdrawalCancelationContext context);
    }
}