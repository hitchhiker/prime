namespace plugins.Services.CryptoCompare.Model
{
    public class CoinSnapshotResult : CryptoCompareResponseBase
    {
        public string Type { get; set; }
        public bool Aggregated { get; set; }

        public CoinSnapshotData Data { get; set; }
    }
}