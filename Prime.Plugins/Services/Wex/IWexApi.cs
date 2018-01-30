using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prime.Plugins.Services.Common;
using RestEase;

namespace Prime.Plugins.Services.Wex
{
    public interface IWexApi : ICommonApiTiLiWe
    {
        [Post("/")]
        Task<WexSchema.WithdrawCoinResponse> WithdrawCoinAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
