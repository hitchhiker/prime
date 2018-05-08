using System.Threading.Tasks;
using Prime.Finance.Wallet.Withdrawal.Confirmation;

namespace Prime.Finance
{
    public interface IWithdrawalConfirmationProvider
    {
        Task<WithdrawalConfirmationResult> ConfirmWithdrawalAsync(WithdrawalConfirmationContext context);
    }
}