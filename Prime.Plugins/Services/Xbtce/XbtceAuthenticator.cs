using System;
using System.Collections.Generic;
using System.Net.Http;
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
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var content = request.Content?.ReadAsStringAsync()?.Result;

            string strPayload = $"{timeStamp}{ApiKey.Key}{ApiKey.Secret}{request.Method}{request.RequestUri}{content}";

            var signature = ToBase64(HashHMACSHA256Hex(strPayload, ApiKey.Secret));
            
            request.Headers.Add("Authorization",$"HMAC {ApiKey.Key}:{ApiKey.Secret}:{timeStamp}:{signature}");
        }
    }
}
