using System.Collections.Generic;

namespace plugins.Services.CryptoCompare.Model
{
    public class CoinSnapshotData
    {
        public string Algorithm { get; set; }
        public string ProofType { get; set; }
        public string BlockNumber { get; set; }
        public string NetHashesPerSecond { get; set; }
        public string TotalCoinsMined { get; set; }
        public string BlockReward { get; set; }
        public CoinSnapshotDataBlock AggregatedData { get; set; }
        public List<CoinSnapshotDataBlock> Exchanges { get; set; }
    }
}