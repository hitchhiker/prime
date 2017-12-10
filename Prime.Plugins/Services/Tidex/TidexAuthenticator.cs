using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;

namespace Prime.Plugins.Services.Tidex
{
    internal class TidexAuthenticator : BaseAuthenticator
    {
        public TidexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        private static readonly long TidexCustomEpochTicks = new DateTime(2017, 12, 1).Ticks;

        protected override long GetNonce()
        {
            return (DateTime.UtcNow.Ticks - TidexCustomEpochTicks) / 10000;
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;

            var prevData = ApiHelpers.DecodeUrlEncodedBody(request.Content.ReadAsStringAsync().Result).ToList();

            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("nonce", GetNonce().ToString()));
            postData.AddRange(prevData);

            var bodyDataEnc = postData.Select(x => $"{x.Key}={x.Value}").ToArray(); 

            var message = string.Join("&", bodyDataEnc);
            var sign = HashHMACSHA512Hex(message, ApiKey.Secret);

            request.Content = new FormUrlEncodedContent(postData);

            headers.Add("KEY", ApiKey.Key);
            headers.Add("Sign", sign);
        }
    }
}
