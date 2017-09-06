namespace Rokolab.BitstampClient.Models
{
    public class BuySellResponse
    {
        public string id { get; set; }
        public string datetime { get; set; }
        public string type { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
    }
}