using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IWithdrawalBase<in TWithdrawalContext> where TWithdrawalContext : WithdrawalPlacementContext
    {
        bool IsFeeIncluded { get; }

        Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(TWithdrawalContext context);
    }
}