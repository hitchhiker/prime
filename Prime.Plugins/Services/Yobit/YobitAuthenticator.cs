using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Yobit
{
    public class YobitAuthenticator : BaseAuthenticator
    {

        public YobitAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            var strForSign = request.Content?.ReadAsStringAsync()?.Result;
            
            var signature = HashHMACSHA512Hex(strForSign, ApiKey.Secret);

            headers.Add("Key", ApiKey.Key);
            headers.Add("Sign", signature);
        }
    }
}
