namespace KrakenApi
{
    public class GetWithdrawStatusResult
    {
        /// <summary>
        /// Name of the withdrawal method used.
        /// </summary>
        public string Method;

        /// <summary>
        /// Asset class.
        /// </summary>
        public string Aclass;

        /// <summary>
        /// Asset X-ISO4217-A3 code.
        /// </summary>
        public string Asset;

        /// <summary>
        /// Reference id.
        /// </summary>
        public string RefId;

        /// <summary>
        /// Method transaction id.
        /// </summary>
        public string Txid;

        /// <summary>
        /// Method transaction information.
        /// </summary>
        public string Info;

        /// <summary>
        /// Amount withdrawn.
        /// </summary>
        public decimal Amount;

        /// <summary>
        /// Fees paid.
        /// </summary>
        public decimal Fee;

        /// <summary>
        /// Unix timestamp when request was made.
        /// </summary>
        public int Time;

        /// <summary>
        /// Status of withdrawal.
        /// </summary>
        public string Status;

        //status-prop = additional status properties(if available).
        //cancel-pending = cancelation requested.
        //canceled = canceled.
        //cancel-denied = cancelation requested but was denied.
        //return = a return transaction initiated by Kraken; it cannot be canceled.
        //onhold = withdrawal is on hold pending review.
    }
}