using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitBay
{
    public class BitBayAuthenticator : BaseAuthenticator
    {
        public BitBayAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var strForSign = request.Content?.ReadAsStringAsync()?.Result;

            var signature = HashHMACSHA512Hex(strForSign, ApiKey.Secret);

            headers.Add("API-Key", ApiKey.Key);
            headers.Add("API-Hash", signature);
        }
    }
}
