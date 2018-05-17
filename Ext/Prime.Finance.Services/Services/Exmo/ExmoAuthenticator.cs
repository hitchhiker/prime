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
            //TODO - SC: Remove hardcoded keys
            string publicKey = "K-d0e5e4dacff774ad56cc2e7f8f33737eb27ff416";
            string secret = "S-64621cfae424107ce9338aa6449f00b997e5e39b";

            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var content = request.Content?.ReadAsStringAsync()?.Result;
            var headers = request.Headers;

            var parameters = string.IsNullOrWhiteSpace(content) ? $"nonce={nonce}" : $"nonce={nonce}&{content}";

            var signature = HashHMACSHA512Hex(parameters,/* ApiKey.Secret*/secret);
            
            request.Content = new StringContent(parameters, Encoding.UTF8, "application/x-www-form-urlencoded");

            headers.Add("Key", /*ApiKey.Key*/ publicKey);
            headers.Add("Sign", signature);
        }
    }
}
