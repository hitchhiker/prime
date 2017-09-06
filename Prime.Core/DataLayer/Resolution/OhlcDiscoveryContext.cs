namespace Prime.Core
{
    public class OhlcDiscoveryContext
    {
        public OhlcDiscoveryContext() { }

        public OhlcDiscoveryContext(OhlcDiscoveryContext context)
        {
            Pair = context.Pair;
            PeggedEnabled = context.PeggedEnabled;
            ConversionEnabled = context.ConversionEnabled;
        }

        public AssetPair Pair { get; set; }

        public bool PeggedEnabled { get; set; }

        public bool ConversionEnabled { get; set; } = true;
    }
}