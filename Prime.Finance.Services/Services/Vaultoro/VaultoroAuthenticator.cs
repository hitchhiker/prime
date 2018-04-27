using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Finance.Services.Services.Vaultoro
{
    public class VaultoroAuthenticator : BaseAuthenticator
    {

        public VaultoroAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.
            
            var headers = request.Headers;

            string path = request.RequestUri.AbsoluteUri;

            path = !path.Contains("?") ? $"{path}?" : $"{path}&";

            string uri = $"{path}nonce={nonce}&apikey={ApiKey.Key}";

            var signature = HashHMACSHA256Hex(uri, ApiKey.Secret).ToLower();

            request.RequestUri = new Uri(uri);

            headers.Add("X-Signature", signature);
        }
    }
}
