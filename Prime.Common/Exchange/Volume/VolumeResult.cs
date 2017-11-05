using Prime.Common.Exchange;

namespace Prime.Common
{
    public class VolumeResult
    {
        public AssetPair Pair { get; set; }
        public decimal Volume { get; set; }
        public VolumePeriod Period { get; set; }
    }
}
