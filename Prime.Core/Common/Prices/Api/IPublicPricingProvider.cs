using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IPublicPricingProvider : IDescribesAssets
    {
        PricingFeatures PricingFeatures { get; }

        Task<MarketPrices> GetPricingAsync(PublicPricesContext context);
    }
}