using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Quoine
{
    public class QuoineAuthenticator : BaseAuthenticator
    {

        public QuoineAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.
        
            var headers = request.Headers;

            string path = request.RequestUri.AbsolutePath;

            var headerBuilder = new StringBuilder("{\"alg\":\"HS256\",\"typ\":\"JWT\"}");
            var payloadBuilder = new StringBuilder("{\"path\":\"" + path + "\",\"nonce\":\"" + nonce + "\", \"token_id\":\"" + ApiKey.Key + "\"}");
            
            var signature = HashHMACSHA256($"{ToBase64(headerBuilder.ToString())}.{ToBase64(payloadBuilder.ToString())}", ApiKey.Secret);

            headers.Add("X-Quoine-API-Version", "2");
            headers.Add("X-Quoine-Auth", $"{ToBase64(headerBuilder.ToString())}.{ToBase64(payloadBuilder.ToString())}.{signature}");
        }
    }
}
