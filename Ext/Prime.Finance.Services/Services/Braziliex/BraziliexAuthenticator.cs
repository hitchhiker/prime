using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Braziliex
{
    public class BraziliexAuthenticator : BaseAuthenticator
    {

        public BraziliexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var parameters = request.Content?.ReadAsStringAsync()?.Result;
            
            var signature = HashHMACSHA512Hex(parameters, ApiKey.Secret);

            headers.Add("Key", ApiKey.Key);
            headers.Add("Sign", signature);
        }
    }
}
