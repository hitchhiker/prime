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
            var clientId = "49041";
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.
            var signatureStr = $"{timeStamp}{clientId}{ApiKey.Key}";

            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            var signature = HashHMACSHA256Hex(signatureStr, ApiKey.Secret).ToUpper();

            if (string.IsNullOrWhiteSpace(parameters))
            {
                request.Content =
                    new StringContent(
                        $"clientId={clientId}&publicKey={ApiKey.Key}&nonce={timeStamp}&signature={signature}",
                        Encoding.UTF8, "application/x-www-form-urlencoded");
            }
            else
            {
                request.Content =
                    new StringContent(
                        $"{parameters}&clientId={clientId}&publicKey={ApiKey.Key}&nonce={timeStamp}&signature={signature}",
                        Encoding.UTF8, "application/x-www-form-urlencoded");
            }
        }
    }
}
