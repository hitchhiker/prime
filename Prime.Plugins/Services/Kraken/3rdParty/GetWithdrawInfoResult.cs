namespace KrakenApi
{
    public class GetWithdrawInfoResult
    {
        /// <summary>
        /// Name of the withdrawal method that will be used
        /// </summary>
        public string Method;

        /// <summary>
        /// Maximum net amount that can be withdrawn right now.
        /// </summary>
        public decimal Limit;

        /// <summary>
        /// Amount of fees that will be paid.
        /// </summary>
        public decimal Fee;
    }
}