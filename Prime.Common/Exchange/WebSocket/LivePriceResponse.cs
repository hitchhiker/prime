namespace Prime.Common
{
    public class LivePriceResponse
    {
        public string SubscriptionId  {get; set;}
        public string ExchangeName { get; set;}
        public string FromCurrency { get; set;}
        public string ToCurrency { get; set;}
        public string Flag { get; set;}
        public double Price { get; set;}
        public string LastUpdate { get; set;}
        public double LastVolume { get; set;}
        public double LastVolumeTo { get; set;}
        public string LastTradeId { get; set;}
        public double Volume24h { get; set;}
        public double Volume24hTo { get; set;}
        public string LastMarket { get; set; }
    }
}