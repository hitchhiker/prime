namespace Prime.Common
{
    /// <summary>
    /// Provides extended withdrawal functionality.
    /// </summary>
    /// <typeparam name="TResult">Type of withdrawal placement result.</typeparam>
    public interface IWithdrawalPlacementProviderExtended<TResult> : IWithdrawalBase<TResult, WithdrawalPlacementContextExtended>
    {
        
    }
}