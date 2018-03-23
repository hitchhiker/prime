using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.NLexch
{
    class NLexchAuthenticator : BaseAuthenticator
    {
        public NLexchAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timeStamp = (long)DateTime.UtcNow.ToUnixTimeStamp() * 1000;

            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            if (parameters == null)
            {
                parameters = request.RequestUri.Query.TrimStart('?');
            }

            string[] arrParameters;

            if (string.IsNullOrWhiteSpace(parameters))
            {
                arrParameters = $"tonce={timeStamp}&access_key={ApiKey.Key}".Split('&');
            }
            else
            {
                arrParameters = $"{parameters}&tonce={timeStamp}&access_key={ApiKey.Key}".Split('&');
            }

            Array.Sort(arrParameters); //Sorts array alphabetically.

            string strToHash = $"{request.Method}|{request.RequestUri.AbsolutePath.Substring(1)}|{string.Join("&", arrParameters)}";

            var signature = HashHMACSHA256Hex(strToHash, ApiKey.Secret);

            request.Content = new StringContent($"{string.Join("&", arrParameters)}&signature={signature}", Encoding.UTF8, "application/x-www-form-urlencoded");
        }
    }
}
