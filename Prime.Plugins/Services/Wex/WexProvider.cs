using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Plugins.Services.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Wex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    // https://wex.nz/api/3/docs
    public class WexProvider : CommonProviderTiLi<IWexApi>
    {
        private const string WexApiVersion = "3";
        private const string WexApiUrl = "https://wex.nz/api/" + WexApiVersion;

        private static readonly ObjectId IdHash = "prime:wex".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        private RestApiClientProvider<IWexApi> ApiProvider { get; }

        public override Network Network { get; } = Networks.I.Get("Wex");

        public override ObjectId Id => IdHash;
        public override IRateLimiter RateLimiter => Limiter;

        protected override RestApiClientProvider<IWexApi> ApiProviderPublic => ApiProvider;
        protected override RestApiClientProvider<IWexApi> ApiProviderPrivate => ApiProvider;

        public WexProvider()
        {
            ApiProvider = new RestApiClientProvider<IWexApi>(WexApiUrl, this, (k) => null);
        }
    }
}
