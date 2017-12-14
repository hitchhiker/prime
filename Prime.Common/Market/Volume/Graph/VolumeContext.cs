namespace Prime.Common
{
    public class VolumeContext
    {
        public bool FlushVolume { get; set; }

        public bool AllowVolumeAssumptions { get; set; }

        public decimal MinimumBtcVolume { get; set; } = 500;
    }
}