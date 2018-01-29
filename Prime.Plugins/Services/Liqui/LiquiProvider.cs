using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Plugins.Services.Base;
using Prime.Utility;

namespace Prime.Plugins.Services.Liqui
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://liqui.io/api
    public partial class LiquiProvider : BaseProvider<ILiquiApi>
    {
        private const string LiquiApiVersion = "3";
        private const string LiquiApiUrl = "https://api.liqui.io/api/" + LiquiApiVersion;
        private const string LiquiApiUrlPrivate = "https://api.liqui.io/tapi";

        private static readonly ObjectId IdHash = "prime:liqui".GetObjectIdHashCode();

        public override Network Network { get; } = Networks.I.Get("Liqui");

        public override ObjectId Id => IdHash;
        protected override RestApiClientProvider<ILiquiApi> ApiProviderPublic { get; }
        protected override RestApiClientProvider<ILiquiApi> ApiProviderPrivate { get; }

        public LiquiProvider() : base()
        {
            ApiProviderPrivate = new RestApiClientProvider<ILiquiApi>(LiquiApiUrlPrivate, this, (k) => new LiquiAuthenticator(k).GetRequestModifierAsync);
            ApiProviderPublic = new RestApiClientProvider<ILiquiApi>(LiquiApiUrl, this, (k) => null);
        }

        public override PricingFeatures PricingFeatures { get; } = new PricingFeatures()
        {
            Single = new PricingSingleFeatures() { CanStatistics = true, CanVolume = true },
        };

        public override async Task<MarketPrices> GetPricingAsync(PublicPricesContext context)
        {
            return await GetPriceAsync(context).ConfigureAwait(false);
        }
    }
}
