namespace KrakenApi
{
    public class FeeInfo
    {
        /// <summary>
        /// Current fee in percent.
        /// </summary>
        public decimal Fee;

        /// <summary>
        /// Minimum fee for pair (if not fixed fee).
        /// </summary>
        public decimal MinFee;

        /// <summary>
        /// Maximum fee for pair (if not fixed fee).
        /// </summary>
        public decimal MaxFee;

        /// <summary>
        /// Next tier's fee for pair (if not fixed fee.  nil if at lowest fee tier).
        /// </summary>
        public decimal NextFee;

        /// <summary>
        /// Volume level of next tier (if not fixed fee.  nil if at lowest fee tier).
        /// </summary>
        public decimal NextVolume;

        /// <summary>
        /// Volume level of current tier (if not fixed fee.  nil if at lowest fee tier).
        /// </summary>
        public decimal TierVolume;
    }
}