using System.Threading.Tasks;
using Prime.Common.Wallet.Withdrawal.Confirmation;

namespace Prime.Common
{
    public interface IWithdrawalConfirmationProvider
    {
        Task<bool> ConfirmWithdrawal(WithdrawalConfirmationContext context);
    }
}