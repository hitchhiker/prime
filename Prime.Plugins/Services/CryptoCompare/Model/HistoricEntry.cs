namespace plugins.Services.CryptoCompare.Model
{
    public class HistoricEntry
    {
        public long time { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double volumefrom { get; set; }
        public double volumeto { get; set; }
    }
}