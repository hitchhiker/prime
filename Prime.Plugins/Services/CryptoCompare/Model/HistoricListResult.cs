using System.Collections.Generic;

namespace plugins.Services.CryptoCompare.Model
{
    public class HistoricListResult : CryptoCompareResponseBase
    {
        public string Type { get; set; }
        public bool Aggregated { get; set; }
        public long TimeTo { get; set; }
        public long TimeFrom { get; set; }
        public bool FirstValueInArray { get; set; }
        public HistoricListConversionType ConversionType { get; set; }

        public List<HistoricEntry> Data { get; set; }
    }
}