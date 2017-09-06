namespace Rokolab.BitstampClient.Models
{
    public class OpenOrderResponse
    {
        public string id { get; set; }
        public string datetime { get; set; }
        public string type { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
        public string reason { get; set; }

        public string type_human
        {
            get
            {
                if (type == "0") return "Buy";
                else if (type == "1") return "Sell";
                else return "Unknown";
            }
        }
    }
}