using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prime.Core
{
    public interface IPublicPricesProvider : INetworkProvider
    {
        Task<PriceLatest> GetLatestPrices(Asset asset, List<Asset> assets);

        Task<PriceLatest> GetLatestPrice(AssetPair asset);
    }
}