namespace Prime.Common
{
    public class PricingFeatures : FeaturesBase<PricingSingleFeatures, PricingBulkFeatures>
    {
        public PricingFeatures() { }

        public PricingFeatures(bool single, bool bulk) : base(single, bulk) { }

        public bool HasVolume => Bulk?.CanVolume == true || Single?.CanVolume == true;
    }
}