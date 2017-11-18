namespace Prime.Common
{
    public class VolumeContext : NetworkProviderContext
    {
        public VolumeContext(AssetPair pair)
        {
            Pair = pair;
        }

        public AssetPair Pair { get; set; }
    }
}
