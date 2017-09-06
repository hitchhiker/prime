using Newtonsoft.Json;

namespace KrakenApi
{
    public class GetDepositMethodsResult
    {
        /// <summary>
        /// Name of deposit method.
        /// </summary>
        public string Method;

        /// <summary>
        /// Maximum net amount that can be deposited right now, or false if no limit
        /// </summary>
        public string Limit;

        /// <summary>
        /// Amount of fees that will be paid.
        /// </summary>
        public string Fee;

        /// <summary>
        /// Whether or not method has an address setup fee (optional).
        /// </summary>
        [JsonProperty(PropertyName = "address-setup-fee")]
        public string AddressSetupFee;

        [JsonProperty(PropertyName = "gen-address")]
        public bool? NewlyGeneratedAddress;

        //[{"method":"EOS","limit":false,"fee":"0.2000000000","address-setup-fee":"2.0000000000","gen-address":true}]}
    }
}