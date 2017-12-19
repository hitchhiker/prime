using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IWithdrawalBase<in TWithdrawalContext> where TWithdrawalContext : WithdrawalPlacementContext
    {
        /// <summary>
        /// If fee is included then you will receive/pay the price that you submit and nothing will be additionally charged.
        /// </summary>
        bool IsFeeIncluded { get; }

        Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(TWithdrawalContext context);
    }
}