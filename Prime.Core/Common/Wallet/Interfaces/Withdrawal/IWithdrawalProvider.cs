namespace Prime.Core
{
    public interface IWithdrawalProvider : 
        IWithdrawalPlacementProvider, IWithdrawalHistoryProvider, IWithdrawalCancelationProvider, IWithdrawalConfirmationProvider
    {
        
    }
}