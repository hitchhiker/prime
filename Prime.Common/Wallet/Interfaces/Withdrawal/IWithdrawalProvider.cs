namespace Prime.Common
{
    public interface IWithdrawalProvider : 
        IWithdrawalPlacementProvider, IWithdrawalHistoryProvider, IWithdrawalCancelationProvider, IWithdrawalConfirmationProvider
    {
        
    }
}