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

            var prevData = request.Content.ReadAsStringAsync().Result.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
            {
                var parts = x.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);

                if(parts.Length != 2)
                    throw new FormatException("Invalid format of post data");

                return new KeyValuePair<string, string>(parts[0], parts[1]);
            }).ToList();

            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("nonce", GetNonce().ToString()));
            postData.AddRange(prevData);

            var message = string.Join("&", postData.Select(x => $"{x.Key}={x.Value}"));
            var sign = HashHMACSHA512Hex(message, ApiKey.Secret);

            request.Content = new FormUrlEncodedContent(postData);

            headers.Add("KEY", ApiKey.Key);
            headers.Add("Sign", sign);
        }
    }
}
