using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IWithdrawalBase<in TWithdrawalContext> where TWithdrawalContext : WithdrawalPlacementContext
    {
        /// <summary>
        /// If fee is included then you will withdraw the price that you submit and nothing will be additionally charged.
        /// Example if "true": withdraw 50 XRP - 1 XRP will be fee and 49 XRP will be withdrawn.
        /// Example if "false": withdraw 50 XRP - 1 XRP will be fee, 51 XRP will be withdrawn and 50 XRP will be received on the other side.
        /// </summary>
        bool IsWithdrawalFeeIncluded { get; }

        Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(TWithdrawalContext context);
    }
}