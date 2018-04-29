using System.Threading.Tasks;
using Prime.Core.Wallet.Withdrawal.Confirmation;

namespace Prime.Core
{
    public interface IWithdrawalConfirmationProvider
    {
        Task<WithdrawalConfirmationResult> ConfirmWithdrawalAsync(WithdrawalConfirmationContext context);
    }
}