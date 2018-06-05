using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.Coinroom
{
    public class CoinroomAuthenticator : BaseAuthenticator
    {
        public CoinroomAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string strForSign = "";
            string content = request.Content?.ReadAsStringAsync().Result;

            if (!string.IsNullOrWhiteSpace(content))
            {
                var arrParams = content.Split('&');
                arrParams = arrParams.Select(s => s.Split('=').Last()).ToArray();
                strForSign = string.Join("", arrParams);
            }

            var signature = HashSHA256($"{strForSign + ApiKey.Secret}").ToLower();

            request.Headers.Add("X-API-Key", ApiKey.Key);

            if (string.IsNullOrWhiteSpace(content))
            {
                request.Content = new StringContent($"sign={signature}", Encoding.UTF8,
                    "application/x-www-form-urlencoded");
            }
            else
            {
                request.Content = new StringContent($"{content}&sign={signature}", Encoding.UTF8,
                    "application/x-www-form-urlencoded");
            }
        }
    }
}
