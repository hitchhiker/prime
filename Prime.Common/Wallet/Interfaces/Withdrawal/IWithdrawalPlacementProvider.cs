namespace Prime.Common
{
    /// <summary>
    /// Provides common withdrawal functionality.
    /// </summary>
    /// <typeparam name="TResult">Type of withdrawal placement result.</typeparam>
    public interface IWithdrawalPlacementProvider<TResult> : IWithdrawalBase<TResult, WithdrawalPlacementContext>
    {
    }
}