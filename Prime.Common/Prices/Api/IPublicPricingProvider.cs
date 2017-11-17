using System.Threading.Tasks;

namespace Prime.Common
{
    public interface IPublicPricingProvider : IDescribesAssets
    {
        PricingFeatures PricingFeatures { get; }

        Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context);
    }
}