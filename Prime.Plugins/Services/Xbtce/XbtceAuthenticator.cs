using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Xbtce
{
    public class XbtceAuthenticator : BaseAuthenticator
    {
        public XbtceAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string webApiId = ApiKey.Extra;

            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var content = request.Content?.ReadAsStringAsync().Result;

            string strPayload = $"{timeStamp}{webApiId}{ApiKey.Key}{request.Method}{request.RequestUri}{content}";

            var signature = HashHMACSHA256(strPayload, ApiKey.Secret);

            request.Headers.Authorization = new AuthenticationHeaderValue("HMAC", string.Format("{0}:{1}:{2}:{3}", webApiId, ApiKey.Key, timeStamp, signature));
        }
    }
}
