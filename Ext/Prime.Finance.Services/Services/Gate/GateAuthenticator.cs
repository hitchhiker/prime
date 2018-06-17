using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Gate
{
    public class GateAuthenticator : BaseAuthenticator
    {
        public GateAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var content = request.Content?.ReadAsStringAsync().Result;

            content = !string.IsNullOrWhiteSpace(content) ? $"{content}&nonce={nonce}" : $"nonce={nonce}";

            var headers = request.Headers;

            var signature = HashHMACSHA512Hex(content, ApiKey.Secret);

            request.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

            headers.Add("KEY", ApiKey.Key);
            headers.Add("SIGN", signature);
        }
    }
}
