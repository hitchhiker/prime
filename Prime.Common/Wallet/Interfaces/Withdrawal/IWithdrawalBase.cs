using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IWithdrawalBase<TResult, TWithdrawalContext> where TWithdrawalContext : WithdrawalPlacementContext
    {
        bool IsFeeIncluded { get; }

        Task<TResult> PlaceWithdrawal(TWithdrawalContext context);
    }
}