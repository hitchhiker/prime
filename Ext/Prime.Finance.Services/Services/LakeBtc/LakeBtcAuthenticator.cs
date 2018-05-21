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
        //TODO - SC - Replace keys
        private string publicKey = "6DKpux1fbGx1uXbP3sXqCujn5goSOHvoQyJKPzGUOz0pxQ";
        private string secret = "avDIVHbrb0IRRRlfotcAvy9ktRHrZLv8itiVECwD63Gxag";

        public LakeBtcAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tonce = (long)(DateTime.UtcNow.ToUnixTimeStamp() * 1000000); // Microseconds.

            var headers = request.Headers;
            var content = request.Content?.ReadAsStringAsync()?.Result;

            var strParams = content?.Replace(":", "=").Replace("\",", "&").Replace("{", "").Replace("}", "").Replace("\"", "");
            
            string strForSign = $"tonce={tonce}&accesskey={/*ApiKey.Key*/publicKey}&requestmethod=post&{strParams}";

            var hash = HashHMACSHA1WithoutBase64(strForSign, /*ApiKey.Secret*/secret);
            var base64Signature = ToBase64($"{/*ApiKey.Key*/publicKey}:{hash}");

            headers.Add("Json-Rpc-Tonce", tonce.ToString());
            headers.Add("Authorization", $"Basic {base64Signature}");

            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
