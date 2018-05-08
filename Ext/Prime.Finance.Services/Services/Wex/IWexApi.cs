using System.Collections.Generic;
using System.Threading.Tasks;
using Prime.Finance.Services.Services.Common;
using RestEase;

namespace Prime.Finance.Services.Services.Wex
{
    public interface IWexApi : ICommonApiTiLiWe
    {
        [Post("/")]
        Task<WexSchema.WithdrawCoinResponse> WithdrawCoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
