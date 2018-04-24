using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Finance.Services.Services.Ccex
{
    public class CcexAuthenticator : BaseAuthenticator
    {

        public CcexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var headers = request.Headers;

            string uri = $"{request.RequestUri.AbsoluteUri}&apikey={ApiKey.Key}&nonce={nonce}";
            request.RequestUri = new Uri(uri);

            var signature = HashHMACSHA512Hex(uri, ApiKey.Secret);

            headers.Add("apisign", signature);
        }
    }
}
