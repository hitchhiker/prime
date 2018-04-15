using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Prime.Common
{
    public class AuthenticatorHmacSha512Basic : BaseAuthenticator
    {
        public AuthenticatorHmacSha512Basic(ApiKey apiKey) : base(apiKey)
        {
        }

        private static readonly long CustomEpochTicks = new DateTime(2018, 04, 1).Ticks;

        protected override long GetNonce()
        {
            return (DateTime.UtcNow.Ticks - CustomEpochTicks) / 100_0000; // 100 ms resolution, enough for up to ~10 years - till Uint32.Max.
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;

            var prevData = ApiHelpers.DecodeUrlEncodedBody(request.Content.ReadAsStringAsync().Result).ToList();

            var nonce = GetNonce();

            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("nonce", nonce.ToString()));
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
