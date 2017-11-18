namespace Prime.Common
{
    public class PricingFeatures
    {
        public PricingFeatures() { }

        public PricingFeatures(bool single, bool bulk)
        {
            Single = single ? new PricingSingleFeatures() : null;
            Bulk = bulk ? new PricingBulkFeatures() : null;
        }

        public bool HasBulk => Bulk != null;

        public bool HasSingle => Single != null;

        public PricingBulkFeatures Bulk { get; set; }

        public PricingSingleFeatures Single { get; set; }

        public bool HasVolume => Bulk?.CanVolume == true || Single?.CanVolume == true;
    }
}