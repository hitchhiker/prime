namespace CryptoFacilities.Api.V1
{
    internal class GetTickerResponse
    {
        public string Result { get; set; }
        public string Error { get; set; }

        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
    }
}