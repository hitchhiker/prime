using System.Threading.Tasks;
using Prime.Common.Wallet.Withdrawal.Cancelation;

namespace Prime.Common
{
    public interface IWithdrawalCancelationProvider<TResult>
    {
        Task<TResult> CancelWithdrawal(WithdrawalCancelationContext context);
    }
}