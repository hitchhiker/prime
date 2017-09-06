namespace Rokolab.BitstampClient.Models
{
    public class TransactionResponse
    {
        public string datetime { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string usd { get; set; }
        public string btc { get; set; }
        public string btc_usd { get; set; }
        public string fee { get; set; }
        public string order_id { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
    }
}