namespace CryptoFacilities.Api.V1
{
    internal class GetCumulativeBidAskResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        //public decimal[][] CumulatedBids { get; set; }
        //public decimal[][] CumulatedAsks { get; set; }

        public string CumulatedBids { get; set; }
        public string CumulatedAsks { get; set; }
    }
}