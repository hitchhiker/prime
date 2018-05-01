using System;
using System.Net.Http;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.HitBtc
{
    public class HitBtcAuthenticator : BaseAuthenticator
    {
        public HitBtcAuthenticator(ApiKey apiKey) : base(apiKey) { }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", $"Basic {ToBase64($"{ApiKey.Key}:{ApiKey.Secret}")}");
        }
    }
}
