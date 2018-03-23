using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.SouthXchange
{
    public class SouthXchangeAuthenticator : BaseAuthenticator
    {
        public SouthXchangeAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModifyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000); // Milliseconds.

            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            var jsonParams = "";

            if (parameters != null)
            {
                jsonParams = parameters.Replace("=", "\":\"").Replace("&", "\",\"");
            }

            var contentBuilder = new StringBuilder("{\"key\":\"" + ApiKey.Key + "\", \"nonce\":\"" + timeStamp + "\"");

            if (string.IsNullOrWhiteSpace(jsonParams))
            {
                contentBuilder.Append("}");
            }
            else
            {
                contentBuilder.Append(",");
                contentBuilder.Append(jsonParams.Replace("{","").Replace("}",""));
                contentBuilder.Append("}");
                contentBuilder.Replace("%2F", "/");
            }

            request.Headers.Add("Hash", HashHMACSHA512Hex(contentBuilder.ToString(), ApiKey.Secret));
            request.Content = new StringContent(contentBuilder.ToString(), Encoding.UTF8, "application/json");
        }
    }
}
