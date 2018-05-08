using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.CoinCorner
{
    public class CoinCornerAuthenticator : BaseAuthenticator
    {

        public CoinCornerAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var nonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var signature = HashHMACSHA256Hex($"{nonce}{ApiKey.Extra}{ApiKey.Key}", ApiKey.Secret);

            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            var contentBuilder = new StringBuilder("{\"API Key\":\"" + ApiKey.Key + "\",\"Signature\":\"" + signature + "\", \"nonce\":\"" + nonce + "\"");

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                parameters = parameters?.Replace("{", "").Replace("}", "");
                contentBuilder.Append(", ");
                contentBuilder.Append(parameters);
            }

            contentBuilder.Append("}");

            request.Content = new StringContent(contentBuilder.ToString(), Encoding.UTF8, "application/json");
        }
    }
}
