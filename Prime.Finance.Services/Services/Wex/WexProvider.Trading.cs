using System;
using System.Threading.Tasks;
using Prime.Core;
using Prime.Finance.Services.Services.Common;

namespace Prime.Finance.Services.Services.Wex
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    // https://wex.nz/api/3/docs
    // https://wex.nz/tapi/docs
    public partial class WexProvider : IWithdrawalPlacementProvider
    {
        public bool IsWithdrawalFeeIncluded => throw new NotImplementedException();
        public async Task<WithdrawalPlacementResult> PlaceWithdrawalAsync(WithdrawalPlacementContext context)
        {
            var api = ApiProviderPrivate.GetApi(context);

            if(context.HasDescription)
                throw new ApiResponseException("Exchange does not support tags.", this);

            var body = CreatePostBody();
            body.Add("method", ApiMethodsConfig[ApiMethodNamesTiLiWe.WithdrawCoin]);
            body.Add("coinName", context.Amount.Asset.ShortCode);
            body.Add("amount", context.Amount.ToDecimalValue());
            body.Add("address", context.Address.Address);

            var r = await api.WithdrawCoinAsync(body).ConfigureAwait(false);
            CheckResponse(r);

            return new WithdrawalPlacementResult()
            {
                WithdrawalRemoteId = r.return_.tId.ToString()
            };
        }
    }
}
