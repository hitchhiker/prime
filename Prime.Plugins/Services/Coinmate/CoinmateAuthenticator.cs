using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Coinmate
{
    public class CoinmateAuthenticator : BaseAuthenticator
    {

        public CoinmateAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //TODO: SC - get client ID from ApiKey.Extra
            var clientId = ApiKey.Extra;
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.
            var signatureStr = $"{timeStamp}{clientId}{ApiKey.Key}";

            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            var signature = HashHMACSHA256Hex(signatureStr, ApiKey.Secret).ToUpper();

            var content = $"clientId={clientId}&publicKey={ApiKey.Key}&nonce={timeStamp}&signature={signature}";

            if (!string.IsNullOrWhiteSpace(parameters))
                content = $"{parameters}" + content;
            
            request.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }
    }
}
