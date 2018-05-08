using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Luno
{
    public class LunoAuthenticator : BaseAuthenticator
    {

        public LunoAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", $"Basic {ToBase64($"{ApiKey.Key}:{ApiKey.Secret}")}");
        }
    }
}
