using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Prime.Core;

namespace Prime.Finance.Services.Services.Gatecoin
{
    public class GatecoinAuthenticator : BaseAuthenticator
    {
        public string publicKey = "HjXdYxHwMl0gPhO6YB6t3xL6Vyr45gX";
        public string secret = "410585562AE51057E6F0FA8B56AAC7E9";

        public GatecoinAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = GetUnixEpochNonceSeconds(); // Seconds.

            var headers = request.Headers;

            var strToSign = $"{request.Method}{request.RequestUri}{(request.Method == HttpMethod.Get ? "" : "application/json")}{nonce}";
            var signature = HashHMACSHA256(strToSign.ToLower(), /*ApiKey.Secret*/secret);

            headers.Add("API_PUBLIC_KEY", /*ApiKey.Key*/publicKey);
            headers.Add("API_REQUEST_SIGNATURE", signature);
            headers.Add("API_REQUEST_DATE", nonce.ToString());
        }
    }
}