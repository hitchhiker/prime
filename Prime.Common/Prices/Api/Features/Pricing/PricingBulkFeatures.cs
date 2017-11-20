namespace Prime.Common
{
    public class PricingBulkFeatures : PricingFeaturesItemBase
    {
        public bool SupportsMultipleQuotes { get; set; } = true;

        public bool CanReturnAll { get; set; }
    }
}