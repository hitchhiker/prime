using System.Threading.Tasks;

namespace Prime.Finance
{
    public interface IPublicPricingProvider : IDescribesAssets
    {
        PricingFeatures PricingFeatures { get; }

        Task<MarketPrices> GetPricingAsync(PublicPricesContext context);
    }
}