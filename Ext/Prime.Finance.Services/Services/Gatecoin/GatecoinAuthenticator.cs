using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Gatecoin
{
    public class GatecoinAuthenticator : BaseAuthenticator
    {
        public GatecoinAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var headers = request.Headers;

            var signature = ToBase64(HashHMACSHA256Hex($"{request.Method} {request.RequestUri.AbsolutePath}{(request.Method == HttpMethod.Get ? "" : "application/json")}{nonce}".ToLower(), ApiKey.Secret));

            headers.Add("API_PUBLIC_KEY", ApiKey.Key);
            headers.Add("API_REQUEST_SIGNATURE", signature);
            headers.Add("API_REQUEST_DATE", nonce.ToString());
        }
    }
}
