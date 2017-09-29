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
        public BitMexAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        private static readonly long ArbTickEpoch = new DateTime(1990, 1, 1).Ticks;

        protected override long GetNonce()
        {
            return DateTime.UtcNow.Ticks - ArbTickEpoch;
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.PathAndQuery;

            var headers = request.Headers;

            var nonce = GetNonce().ToString(); 
            var message = $"GET{path}{nonce}";
            var signature = HashHMACSHA256Hex(message, ApiKey.Secret);

            headers.Add("api-key", ApiKey.Key);
            headers.Add("api-signature", signature);
            headers.Add("api-nonce", nonce);
        }

    }
}
