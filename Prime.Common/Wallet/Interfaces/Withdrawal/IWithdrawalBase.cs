using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IWithdrawalBase<TWithdrawalContext> where TWithdrawalContext : WithdrawalPlacementContext
    {
        bool IsFeeIncluded { get; }

        Task<WithdrawalPlacementResult> PlaceWithdrawal(TWithdrawalContext context);
    }
}