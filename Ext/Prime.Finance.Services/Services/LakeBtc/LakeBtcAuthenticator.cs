using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Core;

namespace Prime.Finance.Services.Services.LakeBtc
{
    public class LakeBtcAuthenticator : BaseAuthenticator
    {
        public LakeBtcAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000000); // Microseconds.

            var headers = request.Headers;
            var content = request.Content?.ReadAsStringAsync()?.Result;

            var strParams = content?.Replace(":", "=").Replace("\",", "&").Replace("{", "").Replace("}", "").Replace("\"", "");
            
            string strForSign = $"tonce={tonce}&accesskey={ApiKey.Key}&requestmethod=post&{strParams}";

            var hash = HashHMACSHA1WithoutBase64(strForSign, ApiKey.Secret);
            var base64Signature = ToBase64($"{ApiKey.Key}:{hash}");

            headers.Add("Json-Rpc-Tonce", tonce.ToString());
            headers.Add("Authorization", $"Basic {base64Signature}");

            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
