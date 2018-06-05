using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Exmo
{
    public class ExmoAuthenticator : BaseAuthenticator
    {
        public ExmoAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var content = request.Content?.ReadAsStringAsync()?.Result;
            var headers = request.Headers;

            var parameters = string.IsNullOrWhiteSpace(content) ? $"nonce={nonce}" : $"nonce={nonce}&{content}";

            var signature = HashHMACSHA512Hex(parameters, ApiKey.Secret);
            
            request.Content = new StringContent(parameters, Encoding.UTF8, "application/x-www-form-urlencoded");

            headers.Add("Key", ApiKey.Key);
            headers.Add("Sign", signature);
        }
    }
}
