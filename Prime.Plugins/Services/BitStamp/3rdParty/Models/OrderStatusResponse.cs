namespace Rokolab.BitstampClient.Models
{
    public class OrderStatusResponse
    {
        public string status { get; set; }
        public Transaction[] transactions { get; set; }
    }
}