namespace Prime.Finance
{
    public interface IWithdrawalProvider : 
        IWithdrawalPlacementProvider, IWithdrawalHistoryProvider, IWithdrawalCancelationProvider, IWithdrawalConfirmationProvider
    {
        
    }
}