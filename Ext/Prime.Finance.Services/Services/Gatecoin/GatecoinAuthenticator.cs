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
        //TODO - SC: Remove hard-coded keys
        private const string key = "HjXdYxPHwMl0gPhO6YB6t3xL6Vyr45gX";
        private const string secret = "410585562AE51057E6F0FA8B56AAC7E9";

        public GatecoinAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var headers = request.Headers;

            var strToSign =$"{request.Method} {request.RequestUri}{(request.Method == HttpMethod.Get ? "" : "application/json")}{nonce}";
            var signature = HashHMACSHA256(strToSign.ToLower(), secret/*ApiKey.Secret*/);

            headers.Add("API_PUBLIC_KEY", /*ApiKey.Key*/key);
            headers.Add("API_REQUEST_SIGNATURE", signature);
            headers.Add("API_REQUEST_DATE", nonce.ToString());
        }
    }
}
