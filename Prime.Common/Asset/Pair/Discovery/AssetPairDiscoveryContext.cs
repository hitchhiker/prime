namespace Prime.Common
{
    public class AssetPairDiscoveryContext
    {
        public AssetPairDiscoveryContext() { }

        public AssetPairDiscoveryContext(AssetPairDiscoveryContext context)
        {
            Pair = context.Pair;
            PeggedEnabled = context.PeggedEnabled;
            ConversionEnabled = context.ConversionEnabled;
        }

        public Network Network { get; set; }
        
        public AssetPair Pair { get; set; }

        public bool PeggedEnabled { get; set; }

        public bool ConversionEnabled { get; set; } = true;

        public bool ReversalEnabled { get; set; } = false;
    }
}