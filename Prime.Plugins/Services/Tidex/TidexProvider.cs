using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Prime.Common;
using Prime.Plugins.Services.Base;
using Prime.Plugins.Services.Kraken.Converters;
using Prime.Plugins.Services.Tidex.Converters;
using Prime.Utility;

namespace Prime.Plugins.Services.Tidex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://tidex.com/public-api
    public partial class TidexProvider : BaseProvider<ITidexApi>
    {
        private const string TidexApiVersion = "3";
        private const string TidexApiUrlPublic = "https://api.tidex.com/api/" + TidexApiVersion;
        private const string TidexApiUrlPrivate = "https://api.tidex.com/tapi";

        private static readonly ObjectId IdHash = "prime:tidex".GetObjectIdHashCode();

        public override Network Network { get; } = Networks.I.Get("Tidex");
        public override ObjectId Id => IdHash;
        protected override RestApiClientProvider<ITidexApi> ApiProviderPublic { get; }
        protected override RestApiClientProvider<ITidexApi> ApiProviderPrivate { get; }

        public TidexProvider()
        {
            ApiProviderPublic = new RestApiClientProvider<ITidexApi>(TidexApiUrlPublic, this, (k) => null);
            ApiProviderPrivate = new RestApiClientProvider<ITidexApi>(TidexApiUrlPrivate, this, (k) => new TidexAuthenticator(k).GetRequestModifierAsync);
        }
    }
}
