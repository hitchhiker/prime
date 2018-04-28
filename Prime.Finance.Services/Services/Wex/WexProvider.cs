﻿using System;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Finance.Services.Services.Common;

namespace Prime.Finance.Services.Services.Wex
{
    /// <author email="scaruana_prime@outlook.com">Sean Caruana</author>
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://wex.nz/api/3/docs
    // https://wex.nz/tapi/docs
    public partial class WexProvider : CommonProviderTiLiWe<IWexApi>
    {
        private const string WexApiVersion = "3";
        private const string WexApiUrlPublic = "https://wex.nz/api/" + WexApiVersion;
        private const string WexApiUrlPrivate = "https://wex.nz/tapi";

        private static readonly ObjectId IdHash = "prime:wex".GetObjectIdHashCode();

        private static readonly IRateLimiter Limiter = new NoRateLimits();

        public override Network Network { get; } = Networks.I.Get("Wex");

        public override ObjectId Id => IdHash;
        public override IRateLimiter RateLimiter => Limiter;

        protected override RestApiClientProvider<IWexApi> ApiProviderPublic { get; }
        protected override RestApiClientProvider<IWexApi> ApiProviderPrivate { get; }

        public WexProvider()
        {
            ApiProviderPublic = new RestApiClientProvider<IWexApi>(WexApiUrlPublic, this, (k) => null);
            ApiProviderPrivate = new RestApiClientProvider<IWexApi>(WexApiUrlPrivate, this, (k) => new WexAuthenticator(k).GetRequestModifierAsync);

            // Add new methods that support exchange.
            ApiMethodsConfig.Add(ApiMethodNamesTiLiWe.WithdrawCoin, "WithdrawCoin");
            ApiMethodsConfig.Add(ApiMethodNamesTiLiWe.GetInfo, "getInfo");

            // Update existing common methods that support exchange.
            ApiMethodsConfig[ApiMethodNamesTiLiWe.OrderInfo] = "OrderInfo";
        }

        protected override void CheckResponse<T>(CommonSchemaTiLiWe.BaseResponse<T> r)
        {
            if (r.error.IndexOf("no trades", StringComparison.OrdinalIgnoreCase) >= 0 ||
                r.error.IndexOf("no orders", StringComparison.OrdinalIgnoreCase) >= 0)
                return;
            base.CheckResponse(r);
        }

        public override async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            var body = CreatePostBody();
            body.Add("method", ApiMethodsConfig[ApiMethodNamesTiLiWe.GetInfo]);

            var r = await api.GetUserInfoAsync(body).ConfigureAwait(false);

            CheckResponse(r);

            var balances = new BalanceResults(this);

            foreach (var fund in r.return_.funds)
            {
                var c = fund.Key.ToAsset(this);
                balances.Add(c, fund.Value, 0);
            }

            return balances;
        }
    }
}
