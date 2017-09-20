namespace Prime.Core
{
    public class PairProviderDiscoveryContext
    {
        public PairProviderDiscoveryContext() { }

        public PairProviderDiscoveryContext(PairProviderDiscoveryContext context)
        {
            Pair = context.Pair;
            PeggedEnabled = context.PeggedEnabled;
            ConversionEnabled = context.ConversionEnabled;
        }

        public Network Network { get; set; }
        
        public AssetPair Pair { get; set; }

        public bool PeggedEnabled { get; set; }

        public bool ConversionEnabled { get; set; } = true;

        public bool AllowNormalisation { get; set; } = false;
    }
}