using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Coinmate
{
    public class CoinmateAuthenticator : BaseAuthenticator
    {
        public CoinmateAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var clientId = ApiKey.Extra;
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.
            var signatureStr = $"{timeStamp}{clientId}{ApiKey.Key}";

            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            var signature = HashHMACSHA256Hex(signatureStr, ApiKey.Secret).ToUpper();

            var content = $"clientId={clientId}&publicKey={ApiKey.Key}&nonce={timeStamp}&signature={signature}";

            if (!string.IsNullOrWhiteSpace(parameters))
                content = $"{parameters}&{content}";

            request.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

            //if (string.IsNullOrWhiteSpace(parameters))
            //{
            //    request.Content =
            //        new StringContent(
            //            $"clientId={clientId}&publicKey={ApiKey.Key}&nonce={timeStamp}&signature={signature}",
            //            Encoding.UTF8, "application/x-www-form-urlencoded");
            //}
            //else
            //{
            //    request.Content =
            //        new StringContent(
            //            $"{parameters}&clientId={clientId}&publicKey={ApiKey.Key}&nonce={timeStamp}&signature={signature}",
            //            Encoding.UTF8, "application/x-www-form-urlencoded");
            //}

            //request.Content = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }
    }
}
