using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.Plugins.Services.BitMex
{
    public class BitMexAuthenticator : BaseAuthenticator
    {
        #region Secrets

        public const String Key = "ox6GznvJPLW3Op5SWUTtycfo";
        public const String Secret = "2qGGyjRGJm7w8S0a5TQt42KSEq-EDqUv6SiOy9pwE793nHpr";

        #endregion

        public BitMexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        private static long _nonceReference = new DateTime(2015, 1, 1).Ticks;

        protected override long GetNonce()
        {
            return DateTime.UtcNow.Ticks - _nonceReference;
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.PathAndQuery;

            var headers = request.Headers;

            var nonce = GetNonce().ToString(); 
            var message = $"GET{path}{nonce}";
            var signature = HashHMACSHA256Hex(message, Secret);

            headers.Add("api-key", Key);
            headers.Add("api-signature", signature);
            headers.Add("api-nonce", nonce);
        }

    }
}
