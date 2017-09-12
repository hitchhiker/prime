namespace Prime.Core
{
    public class OhlcProviderFinderContext
    {
        public OhlcProviderFinderContext() { }

        public OhlcProviderFinderContext(OhlcProviderFinderContext context)
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