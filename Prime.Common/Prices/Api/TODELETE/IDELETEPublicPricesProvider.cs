using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Common.Exchange;

namespace Prime.Common
{
    [Obsolete]
    public interface IDELETEPublicPricesProvider : IDELETEPublicAssetPricesProvider
    {
        Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context);
    }
}